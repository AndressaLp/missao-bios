using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.Interaction
{
    public partial class SalaMissao : Node2D
    {
        [Export] public string MissaoConcluirAoEntrar = "";

        public override void _Ready()
        {
            var gm = GetNodeOrNull<GameManager>("/root/GameManager");
        if (gm == null) return;

        gm.SalaAtual = SceneFilePath;

        if (!string.IsNullOrEmpty(MissaoConcluirAoEntrar))
            gm.ConcluirMissao(MissaoConcluirAoEntrar);

        GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
        }
    }
}