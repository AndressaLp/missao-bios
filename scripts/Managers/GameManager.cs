using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace MissaoBios.scripts.Managers
{
    public partial class GameManager : Node
    {
        public float ProgressoEstacao { get; private set; } = 0f;
        public string ProximoPontoEntrada { get; set; } = "";

        public void AvancarProgresso(float valor)
        {
            ProgressoEstacao += valor;
        }
    }
}