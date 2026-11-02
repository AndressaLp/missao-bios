using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using System.Text.Json;
using MissaoBios.scripts.Puzzle.Laboratorio;

namespace MissaoBios.scripts.Managers
{
    public partial class PuzzleManager : Node
    {
        private List<Microrganismo> _todosMicrorganismos = new();
        private SorteioService<Microrganismo> _sorteio;

        public List<AmostraEmJogo> AmostrasAtuais { get; private set; } = new();
        public bool RodadaFinalizada { get; set; }
        private List<Substancia> _substancias = new();

        public List<PacienteEmJogo> PacientesAtuais { get; private set; } = new();
        public bool RodadaEnfermariaFinalizada { get; set; }
        public int RodadasEnfermaria { get; set; }

        public Dictionary<string, string> MapaIngredientes { get; private set; } = new();

        public Substancia ObterSubstancia(string id) => _substancias.Find(s => s.Id == id);

        public Microrganismo ObterMicrorganismo(string id) => _todosMicrorganismos.Find(m => m.Id == id);

        public override void _Ready()
        {
            CarregarDados();
            _sorteio = new SorteioService<Microrganismo>(_todosMicrorganismos);
        }

        private void CarregarDados()
        {
            using var arquivo = FileAccess.Open("res://data/microrganismosDB.json", FileAccess.ModeFlags.Read);
            string json = arquivo.GetAsText();

            var opcoesJson = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dados = JsonSerializer.Deserialize<DadosJogo>(json, opcoesJson);
            _todosMicrorganismos = dados?.Microrganismos ?? new List<Microrganismo>();
            _substancias = dados?.SubstanciasCura ?? new List<Substancia>();
        }

        public void GarantirRodadaIniciada()
        {
            if (AmostrasAtuais.Count == 0)
            {
                AmostrasAtuais = new List<AmostraEmJogo>();
                for (int i = 0; i < 3; i++)
                    AmostrasAtuais.Add(new AmostraEmJogo { Dados = _sorteio.Sortear() });
            }
        }

        public bool TodasAsAmostrasResolvidas() =>
            AmostrasAtuais.Count > 0 && AmostrasAtuais.All(a => a.Resolvida);

        public bool AlgumaAmostraResolvida() =>
            AmostrasAtuais.Any(a => a.Resolvida);

        public void ReiniciarEstado()
        {
            ReiniciarRodadaLaboratorio();
            ReiniciarRodadaEnfermaria();
            RodadasEnfermaria = 0;
        }

        public void ReiniciarRodadaLaboratorio()
        {
            AmostrasAtuais = new List<AmostraEmJogo>();
            RodadaFinalizada = false;
        }

        public void ReiniciarRodadaEnfermaria()
        {
            PacientesAtuais = new List<PacienteEmJogo>();
            RodadaEnfermariaFinalizada = false;
            MapaIngredientes = new Dictionary<string, string>();
        }

        public bool GarantirRodadaEnfermariaIniciada()
        {
            if (PacientesAtuais.Count > 0) return true;

            if (_sorteio == null || _todosMicrorganismos.Count == 0)
            {
                GD.PrintErr("[PuzzleManager] Sem dados de microrganismos, não dá pra criar os pacientes.");
                return false;
            }

            // Na 1ª rodada, os pacientes foram contaminados pelos mesmos microrganismos do Laboratório.
            bool usarAmostrasDoLab = RodadasEnfermaria == 0 && AmostrasAtuais.Count == 3;

            PacientesAtuais = new List<PacienteEmJogo>();
            for (int i = 0; i < 3; i++)
            {
                var dados = usarAmostrasDoLab ? AmostrasAtuais[i].Dados : _sorteio.Sortear();
                PacientesAtuais.Add(new PacienteEmJogo { Posicao = i + 1, Dados = dados });
            }

            RodadasEnfermaria++;
            return true;
        }

        public void Restaurar(ProgressoJogo p)
        {
            ReiniciarEstado();

            foreach (var salva in p.Amostras ?? new List<AmostraSalva>())
            {
                var dados = _todosMicrorganismos.Find(m => m.Id == salva.MicrorganismoId);
                if (dados == null) continue;

                AmostrasAtuais.Add(new AmostraEmJogo
                {
                    Dados = dados,
                    Resolvida = salva.Resolvida,
                    Resposta = Enum.TryParse<QualidadeResposta>(salva.Resposta, out var q) ? q : null,
                    DicasUsadas = salva.DicasUsadas,
                });
            }

            RodadaFinalizada = p.RodadaFinalizada;
            PacientesAtuais = new List<PacienteEmJogo>();
            foreach (var salvo in p.Pacientes ?? new List<PacienteSalvo>())
            {
                var dados = _todosMicrorganismos.Find(m => m.Id == salvo.MicrorganismoId);
                if (dados == null) continue;

                PacientesAtuais.Add(new PacienteEmJogo
                {
                    Posicao = salvo.Posicao,
                    Dados = dados,
                    Examinado = salvo.Examinado,
                    Curado = salvo.Curado,
                    DicasUsadas = salvo.DicasUsadas,
                    Tentativas = salvo.Tentativas,
                });
            }
            RodadaEnfermariaFinalizada = p.RodadaEnfermariaFinalizada;
            RodadasEnfermaria = p.RodadasEnfermaria;
            MapaIngredientes = new Dictionary<string, string>(p.MapaIngredientes ?? new Dictionary<string, string>());
        }

        // Um ponto de coleta por ingrediente: 3 em cada sala.
        public static readonly string[] PontosDeIngrediente =
        {
            "hangar_1", "hangar_2", "hangar_3",
            "painel_1", "painel_2", "painel_3",
            "lab_1", "lab_2", "lab_3",
            "enf_1", "enf_2", "enf_3",
        };

        private void GarantirMapaIngredientes()
        {
            if (MapaIngredientes.Count > 0 || _substancias.Count == 0) return;

            var ids = _substancias.Select(s => s.Id).ToList();
            var rnd = new Random();
            for (int i = ids.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (ids[i], ids[j]) = (ids[j], ids[i]);
            }

            if (ids.Count > PontosDeIngrediente.Length)
            GD.PrintErr("[PuzzleManager] Há mais ingredientes do que pontos de coleta! Alguns não vão aparecer.");

            for (int i = 0; i < ids.Count && i < PontosDeIngrediente.Length; i++)
                MapaIngredientes[PontosDeIngrediente[i]] = ids[i];
        }

        public Substancia ObterSubstanciaDoPonto(string pontoId)
        {
            GarantirMapaIngredientes();
            return MapaIngredientes.TryGetValue(pontoId, out var id) ? ObterSubstancia(id) : null;
        }
    }
}