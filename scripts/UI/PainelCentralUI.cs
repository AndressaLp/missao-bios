using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.UI
{
	public partial class PainelCentralUI : CanvasLayer
	{
		private ProgressBar _barraProgresso;
		private GameManager _gameManager;

		public override void _Ready()
		{
			Layer = 15;
			Visible = false;

			_barraProgresso = GetNodeOrNull<ProgressBar>("Painel/BarraProgresso");
			_gameManager = GetNodeOrNull<GameManager>("/root/GameManager");

			var botaoFechar = GetNodeOrNull<Button>("Painel/BotaoFechar");
			if (botaoFechar != null) botaoFechar.Pressed += Fechar;
			else GD.PrintErr("[PainelCentralUI] Nó não encontrado: 'Painel/BotaoFechar'");
		}

		public void Abrir()
		{
			if (_barraProgresso != null && _gameManager != null)
				_barraProgresso.Value = _gameManager.ProgressoEstacao;

			Visible = true;
			GetTree().Paused = true;
		}

		private void Fechar()
		{
			Visible = false;
			GetTree().Paused = false;
		}
	}
}
