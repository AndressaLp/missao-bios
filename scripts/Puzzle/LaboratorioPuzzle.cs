using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace MissaoBios.scripts.Puzzle
{
    // Junta todas as amostras do Laboratório e verifica quando o puzzle inteiro foi resolvido.
    public partial class LaboratorioPuzzle : Node
    {
        [Export] public NodePath[] CaminhosAmostras;

        private List<AmostraController> _amostras = new();

        public override void _Ready()
        {
            if (CaminhosAmostras == null) return;

            foreach (var caminho in CaminhosAmostras)
            {
                _amostras.Add(GetNode<AmostraController>(caminho));
            }
        }

        public bool PuzzleCompleto()
        {
            // sem amostras configuradas nao tem como o puzzle estar completo
            if (_amostras.Count == 0) return false;

            int resolvidas = 0;

            foreach (var amostra in _amostras)
            {
                if (amostra.EstaResolvida())
                {
                    resolvidas++;
                }
            }

            return resolvidas == _amostras.Count;
        }
    }
}
