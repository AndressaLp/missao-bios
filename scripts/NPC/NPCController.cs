using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Interaction;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.NPC
{
    public partial class NPCController : Interactable
    {
        [Export] public string DialogoId = "";
        [Export] public bool MarcarMudancaParaLabAoFinalizar = false;
        [Export] public string MissaoConcluirAoFinalizar = "";

        public override void _Ready()
        {
            var gameManager = GetNodeOrNull<GameManager>("/root/GameManager");
            if (gameManager != null && gameManager.KaelMudouParaLaboratorio &&
                (DialogoId == "kael_hangar" || DialogoId == "kael_painel"))
            {
                QueueFree();
            }
        }

        public override void Interagir()
        {
            var dialogueManager = GetNode<DialogueManager>("/root/DialogueManager");
            var gameManager = GetNode<GameManager>("/root/GameManager");

            void AoFechar()
            {
                dialogueManager.DialogoEncerrado -= AoFechar;
                if (MarcarMudancaParaLabAoFinalizar) gameManager.KaelMudouParaLaboratorio = true;
                if (!string.IsNullOrEmpty(MissaoConcluirAoFinalizar))
                    gameManager.ConcluirMissao(MissaoConcluirAoFinalizar);
            }

            dialogueManager.DialogoEncerrado += AoFechar;
            dialogueManager.IniciarDialogo(DialogoId);
        }
    }
}