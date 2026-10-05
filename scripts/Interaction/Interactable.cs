using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace MissaoBios.scripts.Interaction
{
    // Classe base para qualquer coisa com que o jogador possa interagir com a tecla E.
    // Porta e NPC vão herdar dela.
    public partial class Interactable : Area2D
    {
        public virtual void Interagir()
        {
            // Cada tipo (porta, NPC) sobrescreve isso com seu próprio comportamento.
        }
    }
}