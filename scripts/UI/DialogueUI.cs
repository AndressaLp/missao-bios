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
        private DialogueManager _dialogueManager;

        public override void _Ready()
        {
            Layer = 20; // acima do HUD e do menu de pausa
            Visible = false;

            _labelTexto = GetNode<Label>("Painel/LabelTexto");
            _dialogueManager = GetNode<DialogueManager>("/root/DialogueManager");
            _dialogueManager.FalaAtualizada += OnFalaAtualizada;
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
            _labelTexto.Text = texto;
        }

        private void OnDialogoEncerrado()
        {
            Visible = false;
        }
    }
}