using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using System.Text.Json;

namespace MissaoBios.scripts.Managers
{
    public class PerfilJogador
    {
        public string Id { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Personagem { get; set; } = "menino"; // "menino" ou "menina"
        public float ProgressoEstacao { get; set; } = 0f;
        public string UltimoAcesso { get; set; } = "";
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
    }
}