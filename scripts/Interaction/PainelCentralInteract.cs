using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.UI;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.Interaction
{
    public partial class PainelCentralInteract : Interactable
    {
        private ProgressBar _barra;
        private GameManager _gameManager;

        public override void _Ready()
        {
            _barra = GetNodeOrNull<ProgressBar>("BarraProgresso");
            if (_barra == null) GD.PrintErr("[PainelCentralInteract] Nó não encontrado: 'BarraProgresso'");
            _gameManager = GetNodeOrNull<GameManager>("/root/GameManager");
        }

        public override void _Process(double delta)
        {
            if (_barra != null && _gameManager != null)
                _barra.Value = _gameManager.ProgressoEstacao;
        }

        public override void Interagir()
        {
            GetNode<PainelCentralUI>("/root/PainelCentralUIAutoload").Abrir();
        }
    }
}