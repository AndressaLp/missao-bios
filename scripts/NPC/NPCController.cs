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

        public override void Interagir()
        {
            GetNode<DialogueManager>("/root/DialogueManager").IniciarDialogo(DialogoId);
        }
    }
}