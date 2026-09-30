using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.UI
{
    public partial class HUDController : CanvasLayer
    {
        private Label _labelNomeJogador;
        private Label _labelMissoes;
        private Button _botaoSom;
        private AudioManager _audioManager;

        public override void _Ready()
        {
            _labelNomeJogador = ObterNo<Label>("LabelNomeJogador");
            _labelMissoes = ObterNo<Label>("PainelMissoes/ScrollContainer/LabelMissoes");
            _audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");

            ConectarBotao("BotoesSecundarios/BotaoPausa", AbrirPausa);

            _botaoSom = ObterNo<Button>("BotoesSecundarios/BotaoSom");
            if (_botaoSom != null) _botaoSom.Pressed += AlternarSom;

            var saveManager = GetNodeOrNull<SaveManager>("/root/SaveManager");
            if (saveManager?.PerfilAtivo != null && _labelNomeJogador != null)
                _labelNomeJogador.Text = saveManager.PerfilAtivo.Nome;

            // Placeholder até o GameManager enviar os objetivos reais
            AtualizarMissoes(new List<string>
            {
                "Fale com o Dr. Kael",
                "Observe as amostras no microscópio",
                "Descubra o método certo"
            });
        }

        private T ObterNo<T>(string caminho) where T : Node
        {
            var no = GetNodeOrNull<T>(caminho);
            if (no == null)
                GD.PrintErr($"[HUD] Nó não encontrado: '{caminho}'");
            return no;
        }

        private void ConectarBotao(string caminho, Action acao)
        {
            var botao = ObterNo<Button>(caminho);
            if (botao != null) botao.Pressed += acao;
        }

        public void AtualizarMissoes(List<string> objetivos)
        {
            if (_labelMissoes == null) return;

            string texto = "";
            for (int i = 0; i < objetivos.Count; i++)
                texto += $"{i + 1}. {objetivos[i]}\n";

            _labelMissoes.Text = texto;
        }

        private void AbrirPausa()
        {
            GetNode<PauseMenuController>("/root/PauseMenu").Abrir();
        }

        private void AlternarSom()
        {
            _audioManager.AlternarSom();
            _botaoSom.Text = _audioManager.SomAtivado ? "🔊" : "🔇";
        }
    }
}