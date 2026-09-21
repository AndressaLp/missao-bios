using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.Puzzle
{
    // Permite observar uma amostra de perto antes de testar um método nela.
    public partial class MicroscopioController : Node2D
    {
        private AmostraController _amostraAtual;

        public void Observar(AmostraController amostra)
        {
            _amostraAtual = amostra;
            AtivarZoom();
        }

        // Desliga o zoom do microscópio
        private void AtivarZoom()
        {
            Scale = new Vector2(1.5f, 1.5f);
        }

        public void SairDoMicroscopio()
        {
            _amostraAtual = null;
            Scale = Vector2.One;
        }
    }
}