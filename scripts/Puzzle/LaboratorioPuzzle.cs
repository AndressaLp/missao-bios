using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.Puzzle
{
    // Junta todas as amostras do Laboratório e verifica quando o puzzle inteiro foi resolvido.
    public partial class LaboratorioPuzzle : Node
    {
        [Export] public NodePath[] CaminhosAmostras;

        private List<AmostraController> _amostras = new();
        private int _totalAmostras = 3;

        public override void _Ready()
        {
            foreach (var caminho in CaminhosAmostras)
            {
                _amostras.Add(GetNode<AmostraController>(caminho));
            }
        }

        public bool PuzzleCompleto()
        {
            int resolvidas = 0;

            foreach (var amostra in _amostras)
            {
                if (!amostra.EstaResolvida())
                {
                    resolvidas++;
                }
            }

            return resolvidas >= _totalAmostras;
        }
    }
}