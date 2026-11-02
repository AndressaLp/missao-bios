using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

namespace MissaoBios.scripts.Managers
{
    public class PerfilJogador
    {
        public string Id { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Personagem { get; set; } = "menino"; // "menino" ou "menina"
        public float ProgressoEstacao { get; set; } = 0f;
        public string UltimoAcesso { get; set; } = "";
        public ProgressoJogo Progresso { get; set; }
    }

    // Responsável pelo salvamento local do progresso do jogador (sem backend).
    public partial class SaveManager : Node
    {
        private const string CaminhoSave = "user://saves.json";

        private List<PerfilJogador> _perfis = new List<PerfilJogador>();

        public PerfilJogador PerfilAtivo { get; private set; }

        public override void _Ready()
        {
            CarregarDoDisco();
        }

        public List<PerfilJogador> ListarPerfis() => _perfis;

        public PerfilJogador CriarPerfil(string nome, string personagem)
        {
            var perfil = new PerfilJogador
            {
                Id = Guid.NewGuid().ToString(),
                Nome = nome,
                Personagem = personagem,
                ProgressoEstacao = 0f,
                UltimoAcesso = DateTime.UtcNow.ToString("o")
            };

            _perfis.Add(perfil);
            SalvarNoDisco();
            return perfil;
        }

        public PerfilJogador ObterUltimoPerfil()
        {
            PerfilJogador ultimo = null;
            DateTime maisRecente = DateTime.MinValue;

            foreach (var perfil in _perfis)
            {
                if (DateTime.TryParse(perfil.UltimoAcesso, out DateTime data) && data > maisRecente)
                {
                    maisRecente = data;
                    ultimo = perfil;
                }
            }

            return ultimo;
        }

        public void DefinirPerfilAtivo(PerfilJogador perfil)
        {
            PerfilAtivo = perfil;
            perfil.UltimoAcesso = DateTime.UtcNow.ToString("o");
            SalvarNoDisco();
        }

        private void SalvarNoDisco()
        {
            string json = JsonSerializer.Serialize(_perfis);
            using var arquivo = FileAccess.Open(CaminhoSave, FileAccess.ModeFlags.Write);
            arquivo?.StoreString(json);
        }

        private void CarregarDoDisco()
        {
            if (!FileAccess.FileExists(CaminhoSave))
            {
                _perfis = new List<PerfilJogador>();
                return;
            }

            using var arquivo = FileAccess.Open(CaminhoSave, FileAccess.ModeFlags.Read);
            string json = arquivo?.GetAsText() ?? "[]";

            try
            {
                _perfis = JsonSerializer.Deserialize<List<PerfilJogador>>(json) ?? new List<PerfilJogador>();
            }
            catch
            {
                _perfis = new List<PerfilJogador>();
            }
        }

        public List<PerfilJogador> ListarPerfisRecentes(int quantidade)
        {
            var ordenados = new List<PerfilJogador>(_perfis);

            ordenados.Sort((a, b) =>
            {
                DateTime dataA = DateTime.TryParse(a.UltimoAcesso, out var da) ? da : DateTime.MinValue;
                DateTime dataB = DateTime.TryParse(b.UltimoAcesso, out var db) ? db : DateTime.MinValue;
                return dataB.CompareTo(dataA); // mais recente primeiro
            });

            if (ordenados.Count > quantidade)
                ordenados = ordenados.GetRange(0, quantidade);

            return ordenados;
        }

        public void SalvarProgresso()
        {
            if (PerfilAtivo == null) return;

            var gm = GetNodeOrNull<GameManager>("/root/GameManager");
            var pm = GetNodeOrNull<PuzzleManager>("/root/PuzzleManager");
            if (gm == null || pm == null) return;

            PerfilAtivo.Progresso = new ProgressoJogo
            {
                Sala = gm.SalaAtual,
                ProgressoEstacao = gm.ProgressoEstacao,
                Pontuacao = gm.Pontuacao,
                PontosPorPuzzle = gm.ExportarPontosPorPuzzle(),
                Puzzle1Concluido = gm.Puzzle1Concluido,
                KaelMudouParaLaboratorio = gm.KaelMudouParaLaboratorio,
                RodadaFinalizada = pm.RodadaFinalizada,
                Missoes = gm.Missoes
                    .Select(m => new MissaoSalva { Id = m.Id, Texto = m.Texto, Estado = m.Estado.ToString() })
                    .ToList(),
                Amostras = pm.AmostrasAtuais
                    .Select(a => new AmostraSalva
                    {
                        MicrorganismoId = a.Dados.Id,
                        Resolvida = a.Resolvida,
                        Resposta = a.Resposta?.ToString(),
                        DicasUsadas = a.DicasUsadas,
                    })
                    .ToList(),

                Puzzle2Concluido = gm.Puzzle2Concluido,
                RodadaEnfermariaFinalizada = pm.RodadaEnfermariaFinalizada,
                RodadasEnfermaria = pm.RodadasEnfermaria,
                Pacientes = pm.PacientesAtuais
                    .Select(p => new PacienteSalvo
                    {
                        Posicao = p.Posicao,
                        MicrorganismoId = p.Dados.Id,
                        Examinado = p.Examinado,
                        Curado = p.Curado,
                        DicasUsadas = p.DicasUsadas,
                        Tentativas = p.Tentativas,
                    })
                    .ToList(),
                Inventario = new List<string>(gm.Inventario),
                MapaIngredientes = new Dictionary<string, string>(pm.MapaIngredientes),

                Descobertas = gm.Descobertas.Values
                    .Select(d => new DescobertaSalva { MicrorganismoId = d.MicrorganismoId, Metodo = d.Metodo, Cura = d.Cura })
                    .ToList(),
            };

            SalvarNoDisco();
        }

        // Restaura o jogo a partir do perfil ativo. Devolve a sala salva, ou null se não há progresso.
        public string CarregarProgresso()
        {
            var p = PerfilAtivo?.Progresso;
            if (p == null) return null;

            GetNode<GameManager>("/root/GameManager").Restaurar(p);
            GetNode<PuzzleManager>("/root/PuzzleManager").Restaurar(p);

            return !string.IsNullOrEmpty(p.Sala) && ResourceLoader.Exists(p.Sala) ? p.Sala : null;
        }
    }

    public class ProgressoJogo
    {
        public string Sala { get; set; } = "";
        public float ProgressoEstacao { get; set; }
        public float Pontuacao { get; set; }
        public Dictionary<string, float> PontosPorPuzzle { get; set; } = new();
        public bool Puzzle1Concluido { get; set; }
        public bool KaelMudouParaLaboratorio { get; set; }
        public bool RodadaFinalizada { get; set; }
        public List<MissaoSalva> Missoes { get; set; } = new();
        public List<AmostraSalva> Amostras { get; set; } = new();

        public bool Puzzle2Concluido { get; set; }
        public bool RodadaEnfermariaFinalizada { get; set; }
        public int RodadasEnfermaria { get; set; }
        public List<PacienteSalvo> Pacientes { get; set; } = new();
        public List<string> Inventario { get; set; } = new();
        public Dictionary<string, string> MapaIngredientes { get; set; } = new();
        public List<DescobertaSalva> Descobertas { get; set; } = new();
    }

    public class MissaoSalva
    {
        public string Id { get; set; }
        public string Texto { get; set; }
        public string Estado { get; set; }
    }

    public class AmostraSalva
    {
        public string MicrorganismoId { get; set; }
        public bool Resolvida { get; set; }
        public string Resposta { get; set; }
        public int DicasUsadas { get; set; }
    }

    public class PacienteSalvo
    {
        public int Posicao { get; set; }
        public string MicrorganismoId { get; set; }
        public bool Examinado { get; set; }
        public bool Curado { get; set; }
        public int DicasUsadas { get; set; }
        public int Tentativas { get; set; }
    }

    public class DescobertaSalva
    {
        public string MicrorganismoId { get; set; }
        public bool Metodo { get; set; }   // o jogador já aprendeu como eliminá-lo
        public bool Cura { get; set; }     // o jogador já curou um paciente com ele
    }
}