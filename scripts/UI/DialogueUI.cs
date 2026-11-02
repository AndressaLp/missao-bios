using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.UI
{
    public partial class DialogueUI : CanvasLayer
    {
        private Label _labelTexto;
        private Control _escolhas;
        private Button _botaoSim;
        private Button _botaoNao;
        private DialogueManager _dialogueManager;

        public override void _Ready()
        {
            Layer = 20; // acima do HUD e do menu de pausa
            Visible = false;

            _labelTexto = GetNode<Label>("Painel/LabelTexto");
            _escolhas = GetNodeOrNull<Control>("Escolhas");
            _botaoSim = GetNodeOrNull<Button>("Escolhas/BotaoSim");
            _botaoNao = GetNodeOrNull<Button>("Escolhas/BotaoNao");

            if (_labelTexto == null) GD.PrintErr("[DialogueUI] Nó não encontrado: 'Painel/LabelTexto'");
            if (_escolhas == null || _botaoSim == null || _botaoNao == null)
                GD.PrintErr("[DialogueUI] Nós 'Escolhas/BotaoSim' e 'Escolhas/BotaoNao' não encontrados");

            if (_escolhas != null) _escolhas.Visible = false;
            if (_botaoSim != null) _botaoSim.Pressed += () => Responder(true);
            if (_botaoNao != null) _botaoNao.Pressed += () => Responder(false);

            _dialogueManager = GetNode<DialogueManager>("/root/DialogueManager");
            _dialogueManager.FalaAtualizada += OnFalaAtualizada;
            _dialogueManager.EscolhaSolicitada += OnEscolhaSolicitada;
            _dialogueManager.DialogoEncerrado += OnDialogoEncerrado;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (!Visible) return;

            if (@event.IsActionPressed("interagir"))
            {
                _dialogueManager.AvancarFala();
                GetViewport().SetInputAsHandled(); // evita reabrir o diálogo com o mesmo toque de E
            }
        }

        private void OnFalaAtualizada(string texto)
        {
            Visible = true;
            if (_escolhas != null) _escolhas.Visible = false;
            if (_labelTexto != null) _labelTexto.Text = texto;
        }

        private void OnEscolhaSolicitada(string textoSim, string textoNao)
        {
            if (_escolhas == null || _botaoSim == null || _botaoNao == null)
            {
                _dialogueManager.Escolher(false); // evita travar o jogador se os botões não existirem
                return;
            }

            _botaoSim.Text = textoSim;
            _botaoNao.Text = textoNao;
            _escolhas.Visible = true;
            _botaoSim.GrabFocus();
        }

        private void Responder(bool sim)
        {
            if (_escolhas != null) _escolhas.Visible = false;
            _dialogueManager.Escolher(sim);
        }

        private void OnDialogoEncerrado()
        {
            Visible = false;
            if (_escolhas != null) _escolhas.Visible = false;
        }
    }
}