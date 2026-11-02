using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace MissaoBios.scripts.Managers
{
    public enum EstadoMissao { Oculta, Ativa, Concluida }

    public class Missao
    {
        public string Id;
        public string Texto;
        public EstadoMissao Estado = EstadoMissao.Oculta;
        public double ConcluidaEm; // segundos; 0 = não mostrar em verde
    }

    public partial class GameManager : Node
    {
        [Signal] public delegate void MissoesAtualizadasEventHandler();
        [Signal] public delegate void InventarioAtualizadoEventHandler();

        private readonly Dictionary<string, float> _pontosPorPuzzle = new();

        public float ProgressoEstacao { get; private set; }
        public float Pontuacao { get; private set; }
        public bool Puzzle1Concluido { get; set; }
        public bool KaelMudouParaLaboratorio { get; set; }
        public string ProximoPontoEntrada { get; set; } = "";
        public string SalaAtual { get; set; } = "";
        public List<Missao> Missoes { get; private set; } = new();
        public bool Puzzle2Concluido { get; set; }

        public const int MaxItensInventario = 3;
        public List<string> Inventario { get; private set; } = new();
        public bool InventarioCheio => Inventario.Count >= MaxItensInventario;
        public bool TemItem(string id) => Inventario.Contains(id);

        public bool MissaoConcluida(string id) =>
            Missoes.Find(m => m.Id == id)?.Estado == EstadoMissao.Concluida;

        public override void _Ready() => ReiniciarEstado();

        public void ReiniciarEstado()
        {
            ProgressoEstacao = 0f;
            Pontuacao = 0f;
            Puzzle1Concluido = false;
            Puzzle2Concluido = false;
            Inventario.Clear();
            Descobertas.Clear();
            KaelMudouParaLaboratorio = false;
            ProximoPontoEntrada = "";
            SalaAtual = "";
            _pontosPorPuzzle.Clear();

            Missoes = CriarMissoes();
            Missoes[0].Estado = EstadoMissao.Ativa;
            EmitSignal(SignalName.MissoesAtualizadas);
        }

        private static List<Missao> CriarMissoes() => new()
        {
            new Missao { Id = "falar_kael_hangar", Texto = "Fale com a Dra. Kael" },
            new Missao { Id = "ir_painel_central", Texto = "Vá até o Painel Central(sala Norte)" },                new Missao { Id = "falar_kael_painel", Texto = "Fale com a Dra. Kael no Painel Central" },
            new Missao { Id = "ir_laboratorio", Texto = "Vá até o Laboratório(sala Leste)" },
            new Missao { Id = "falar_kael_lab", Texto = "Fale com a Dra. Kael no Laboratório" },
            new Missao { Id = "analisar_amostras", Texto = "Analise as amostras no microscópio (0/3)" },
            new Missao { Id = "concluir_lab", Texto = "Fale com a Dra. Kael para concluir" },

            new Missao { Id = "ir_enfermaria", Texto = "Vá até a Enfermaria(sala Oeste)" },
            new Missao { Id = "falar_jack", Texto = "Fale com o Dr. Jack" },
            new Missao { Id = "examinar_pacientes", Texto = "Examine os pacientes (0/3)" },
            new Missao { Id = "falar_jack_ingredientes", Texto = "Fale com o Dr. Jack" },
            new Missao { Id = "curar_pacientes", Texto = "Cure os pacientes (0/3)" },
            new Missao { Id = "concluir_enfermaria", Texto = "Fale com o Dr. Jack para concluir" },
        };

        public void AvancarProgresso(float valor) =>
            ProgressoEstacao = Mathf.Clamp(ProgressoEstacao + valor, 0f, 100f);

        public void AlterarPontuacao(float delta, string puzzleId)
        {
            float antes = Pontuacao;
            Pontuacao = Mathf.Clamp(Pontuacao + delta, 0f, 100f);
            _pontosPorPuzzle[puzzleId] = _pontosPorPuzzle.GetValueOrDefault(puzzleId) + (Pontuacao - antes);
        }

        public void ZerarPontosDoPuzzle(string puzzleId)
        {
            Pontuacao = Mathf.Clamp(Pontuacao - _pontosPorPuzzle.GetValueOrDefault(puzzleId), 0f, 100f);
            _pontosPorPuzzle[puzzleId] = 0f;
        }

        public Dictionary<string, float> ExportarPontosPorPuzzle() => new(_pontosPorPuzzle);

        public void ConcluirMissao(string id)
        {
            int idx = Missoes.FindIndex(m => m.Id == id);
            if (idx < 0 || Missoes[idx].Estado == EstadoMissao.Concluida) return;

            double agora = Time.GetTicksMsec() / 1000.0;
            for (int i = 0; i <= idx; i++)
            {
                if (Missoes[i].Estado == EstadoMissao.Concluida) continue;
                Missoes[i].Estado = EstadoMissao.Concluida;
                Missoes[i].ConcluidaEm = (i == idx) ? agora : 0;
            }

            if (idx + 1 < Missoes.Count && Missoes[idx + 1].Estado == EstadoMissao.Oculta)
                Missoes[idx + 1].Estado = EstadoMissao.Ativa;

            EmitSignal(SignalName.MissoesAtualizadas);
            GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
        }

        public void DefinirTextoMissao(string id, string texto)
        {
            var m = Missoes.Find(x => x.Id == id);
            if (m == null) return;
            m.Texto = texto;
            EmitSignal(SignalName.MissoesAtualizadas);
        }

        // Reabre as missões de UM puzzle (da primeira até a última), sem mexer nas dos outros puzzles.
        public void ReabrirMissoes(string primeiraId, string ultimaId, string textoDaPrimeira)
        {
            int ini = Missoes.FindIndex(m => m.Id == primeiraId);
            int fim = Missoes.FindIndex(m => m.Id == ultimaId);
            if (ini < 0 || fim < ini) return;

            for (int i = ini; i <= fim; i++)
            {
                Missoes[i].Estado = (i == ini) ? EstadoMissao.Ativa : EstadoMissao.Oculta;
                Missoes[i].ConcluidaEm = 0;
            }
            Missoes[ini].Texto = textoDaPrimeira;

            EmitSignal(SignalName.MissoesAtualizadas);
            GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
        }

        public void Restaurar(ProgressoJogo p)
        {
            ProgressoEstacao = p.ProgressoEstacao;
            Pontuacao = p.Pontuacao;
            Puzzle1Concluido = p.Puzzle1Concluido;
            Puzzle2Concluido = p.Puzzle2Concluido;
            Inventario = new List<string>(p.Inventario ?? new List<string>());
            KaelMudouParaLaboratorio = p.KaelMudouParaLaboratorio;
            ProximoPontoEntrada = "";
            SalaAtual = p.Sala ?? "";

            _pontosPorPuzzle.Clear();
            if (p.PontosPorPuzzle != null)
                foreach (var par in p.PontosPorPuzzle) _pontosPorPuzzle[par.Key] = par.Value;

            Missoes = CriarMissoes();
            foreach (var salva in p.Missoes ?? new List<MissaoSalva>())
            {
                var m = Missoes.Find(x => x.Id == salva.Id);
                if (m == null) continue;
                m.Texto = salva.Texto;
                if (Enum.TryParse<EstadoMissao>(salva.Estado, out var estado)) m.Estado = estado;
            }

            Descobertas.Clear();
            foreach (var d in p.Descobertas ?? new List<DescobertaSalva>())
                Descobertas[d.MicrorganismoId] = d;

            EmitSignal(SignalName.MissoesAtualizadas);
        }

        public bool AdicionarItem(string id)
        {
            if (InventarioCheio || TemItem(id)) return false;

            Inventario.Add(id);
            EmitSignal(SignalName.InventarioAtualizado);
            GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
            return true;
        }

        public bool RemoverItem(string id)
        {
            if (!Inventario.Remove(id)) return false;

            EmitSignal(SignalName.InventarioAtualizado);
            GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
            return true;
        }

        public void LimparInventario()
        {
            if (Inventario.Count == 0) return;
            Inventario.Clear();
            EmitSignal(SignalName.InventarioAtualizado);
        }

        public Dictionary<string, DescobertaSalva> Descobertas { get; private set; } = new();

        public void RegistrarDescoberta(string microrganismoId, bool metodo, bool cura)
        {
            if (!Descobertas.TryGetValue(microrganismoId, out var d))
            {
                d = new DescobertaSalva { MicrorganismoId = microrganismoId };
                Descobertas[microrganismoId] = d;
            }
            d.Metodo |= metodo;
            d.Cura |= cura;
            GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
        }
    }
}