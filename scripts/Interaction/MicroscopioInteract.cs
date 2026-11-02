using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.UI;

namespace MissaoBios.scripts.Interaction
{
    public partial class MicroscopioInteract : Interactable
    {
        public override void Interagir()
        {
            GetNode<MicroscopioUI>("/root/MicroscopioUIAutoload").Abrir();
        }
    }
}