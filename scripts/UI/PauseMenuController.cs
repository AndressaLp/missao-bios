using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace MissaoBios.scripts.UI
{
    public partial class PauseMenuController : CanvasLayer
    {
        private const string CenaMenuPrincipal = "res://scenes/main/MainMenu.tscn";
        private Panel _painelPausa;
        private Panel _painelComoJogar;

        public override void _Ready()
        {
            Visible = false;
            Layer = 10;

            _painelPausa = ObterNo<Panel>("PainelPausa");
            _painelComoJogar = ObterNo<Panel>("PainelComoJogar");
            _painelComoJogar.Visible = false;

            ConectarBotao("PainelPausa/BotoesPrincipais/BotaoContinuar", Fechar);
            ConectarBotao("PainelPausa/BotoesPrincipais/BotaoComoJogar", AbrirComoJogar);
            ConectarBotao("PainelPausa/BotoesPrincipais/BotaoVoltarMenu", VoltarAoMenu);
            ConectarBotao("PainelComoJogar/BotaoFecharComoJogar", FecharComoJogar);
        }

        private T ObterNo<T>(string caminho) where T : Node
        {
            var no = GetNodeOrNull<T>(caminho);
            if (no == null)
                GD.PrintErr($"[PauseMenu] Nó não encontrado: '{caminho}'");
            return no;
        }

        private void ConectarBotao(string caminho, Action acao)
        {
            var botao = ObterNo<Button>(caminho);
            if (botao != null) botao.Pressed += acao;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (!@event.IsActionPressed("ui_cancel")) return;

            string cenaAtual = GetTree().CurrentScene?.SceneFilePath ?? "";
            if (!Visible && cenaAtual == CenaMenuPrincipal) return;

            if (Visible) Fechar();
            else Abrir();
        }

        public void Abrir()
        {
            Visible = true;
            _painelPausa.Visible = true;
            _painelComoJogar.Visible = false;
            GetTree().Paused = true;
        }

        public void AbrirComoJogarDireto()
        {
            Abrir();
            AbrirComoJogar();
        }

        private void AbrirComoJogar()
        {
            if (_painelPausa != null) _painelPausa.Visible = false;
            if (_painelComoJogar != null) _painelComoJogar.Visible = true;
        }

        private void FecharComoJogar()
        {
            if (_painelComoJogar != null) _painelComoJogar.Visible = false;
            if (_painelPausa != null) _painelPausa.Visible = true;
        }

        private void Fechar()
        {
            Visible = false;
            GetTree().Paused = false;
        }

        private void VoltarAoMenu()
        {
            Visible = false;
            GetTree().Paused = false;
            GetTree().ChangeSceneToFile(CenaMenuPrincipal);
        }
    }
}