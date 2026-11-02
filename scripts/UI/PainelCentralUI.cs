using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;
using MissaoBios.scripts.Puzzle.Laboratorio;
using MissaoBios.scripts.Puzzle.Enfermaria;

namespace MissaoBios.scripts.UI
{
	public partial class PainelCentralUI : CanvasLayer
	{
		private ProgressBar _barraProgresso;
		private GameManager _gameManager;
		private Label _labelPontuacao;
		private RichTextLabel _bancoDados;
		private PuzzleManager _puzzleManager;

		public override void _Ready()
		{
			Layer = 15;
			Visible = false;

			_barraProgresso = GetNodeOrNull<ProgressBar>("Painel/BarraProgresso");
			_gameManager = GetNodeOrNull<GameManager>("/root/GameManager");
			_labelPontuacao = GetNodeOrNull<Label>("Painel/LabelPontuacao");

			var botaoFechar = GetNodeOrNull<Button>("Painel/BotaoFechar");
			if (botaoFechar != null) botaoFechar.Pressed += Fechar;
			else GD.PrintErr("[PainelCentralUI] Nó não encontrado: 'Painel/BotaoFechar'");

			_puzzleManager = GetNodeOrNull<PuzzleManager>("/root/PuzzleManager");
			_bancoDados = GetNodeOrNull<RichTextLabel>("Painel/LabelBancoDados");
			if (_bancoDados == null)
    			GD.PrintErr("[PainelCentralUI] 'Painel/LabelBancoDados' não encontrado (ou ainda não é RichTextLabel).");
			else
			{
    			_bancoDados.BbcodeEnabled = true;
    			_bancoDados.ScrollActive = true;
			}
		}

		public void Abrir()
		{
			if (_labelPontuacao != null && _gameManager != null)
    			_labelPontuacao.Text = $"Pontuação: {_gameManager.Pontuacao:0}";
			if (_barraProgresso != null && _gameManager != null)
				_barraProgresso.Value = _gameManager.ProgressoEstacao;

			if (_bancoDados != null) _bancoDados.Text = MontarBancoDeDados();
			Visible = true;
			GetTree().Paused = true;
		}

		private void Fechar()
		{
			Visible = false;
			GetTree().Paused = false;
		}

		private string MontarBancoDeDados()
		{
    		if (_gameManager == null || _puzzleManager == null || _gameManager.Descobertas.Count == 0)
        		return "Nenhuma descoberta ainda. Analise amostras e cure pacientes para preencher o banco de dados.";

    		var linhas = new List<string>();
    		foreach (var d in _gameManager.Descobertas.Values)
    		{
        		var m = _puzzleManager.ObterMicrorganismo(d.MicrorganismoId);
        		if (m == null) continue;

        		string linha = $"[color=#ffd966]{m.Nome}[/color] ({RegrasPuzzleEnfermaria.TipoAmigavel(m.Tipo)})";
        		if (d.Metodo)
            		linha += $"\n  Eliminar: {RegrasPuzzleLaboratorio.NomeAmigavel(m.MetodoCorreto).ToLower()}";
        		if (d.Cura)
        		{
            		string esp = _puzzleManager.ObterSubstancia(m.CuraIngredientes[0])?.Nome;
            		string sint = _puzzleManager.ObterSubstancia(m.CuraIngredientes[1])?.Nome;
            		linha += $"\n  Cura: {esp} + {sint}";
        		}
        		linhas.Add(linha);
    		}
    		return string.Join("\n", linhas);
		}
	}
}
