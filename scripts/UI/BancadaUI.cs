using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MissaoBios.scripts.Managers;
using MissaoBios.scripts.Puzzle.Enfermaria;

namespace MissaoBios.scripts.UI
{
	public partial class BancadaUI : CanvasLayer
	{
		private const string TextoInicial =
			"Escolha 2 ingredientes: um que combata o microrganismo e outro que alivie o sintoma.";

		private readonly Button[] _botoesItem = new Button[3];
		private Button _botaoMisturar;
		private Button _botaoVoltar;
		private TextureRect _bequer;
		private CpuParticles2D _fumaca;
		private Label _labelJack;

		private PuzzleManager _puzzleManager;
		private GameManager _gameManager;
		private Tween _efeito;
		private Vector2 _posicaoOriginal;
		private Vector2 _escalaOriginal = Vector2.One;

		public override void _Ready()
		{
			Layer = 16;
			ProcessMode = ProcessModeEnum.Always; // funciona com o jogo pausado
			Visible = false;

			_bequer = ObterNo<TextureRect>("Control/Bequer");
			_fumaca = GetNodeOrNull<CpuParticles2D>("Control/Bequer/Fumaca"); // opcional
			_labelJack = ObterNo<Label>("Control/PainelJack/LabelJack");

			_botaoVoltar = ObterNo<Button>("Control/BotaoVoltar");
			if (_botaoVoltar != null) _botaoVoltar.Pressed += Fechar;

			_botaoMisturar = ObterNo<Button>("Control/BotaoMisturar");
			if (_botaoMisturar != null) _botaoMisturar.Pressed += Misturar;

			for (int i = 0; i < 3; i++)
			{
				int indice = i;
				_botoesItem[i] = ObterNo<Button>($"Control/BotaoItem{i + 1}");
				if (_botoesItem[i] == null) continue;
				_botoesItem[i].ToggleMode = true;
				_botoesItem[i].Pressed += () => AoClicarItem(indice);
			}

			if (_bequer != null)
			{
				_posicaoOriginal = _bequer.Position;
				_escalaOriginal = _bequer.Scale;
			}

			_puzzleManager = GetNodeOrNull<PuzzleManager>("/root/PuzzleManager");
			_gameManager = GetNodeOrNull<GameManager>("/root/GameManager");

			IgnorarCliquesDecorativos(this);
		}

		private T ObterNo<T>(string caminho) where T : Node
		{
			var no = GetNodeOrNull<T>(caminho);
			if (no == null) GD.PrintErr($"[BancadaUI] Nó não encontrado: '{caminho}'");
			return no;
		}

		private static void IgnorarCliquesDecorativos(Node no)
		{
			foreach (Node filho in no.GetChildren())
			{
				if (filho is Control c && filho is not BaseButton)
					c.MouseFilter = Control.MouseFilterEnum.Ignore;
				IgnorarCliquesDecorativos(filho);
			}
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (!Visible || !@event.IsActionPressed("ui_cancel")) return;
			Fechar();
			GetViewport().SetInputAsHandled();
		}

		public void Abrir()
		{
			AtualizarBotoes();
			Mensagem(TextoInicial);
			Visible = true;
			GetTree().Paused = true;
		}

		private void Fechar()
		{
			ParaEfeito();
			Visible = false;
			GetTree().Paused = false;
		}

		private void Mensagem(string texto)
		{
			if (_labelJack != null) _labelJack.Text = "Dr. Jack: " + texto;
		}

		private void AtualizarBotoes()
		{
			var itens = _gameManager.Inventario;
			for (int i = 0; i < 3; i++)
			{
				var botao = _botoesItem[i];
				if (botao == null) continue;

				botao.ButtonPressed = false;
				if (i < itens.Count)
				{
					string id = itens[i];
					bool remedio = RegrasPuzzleEnfermaria.EhRemedio(id);
					botao.Text = remedio ? "Remédio pronto" : RegrasPuzzleEnfermaria.NomeDoItem(_puzzleManager, id);
					botao.Disabled = remedio; // remédio não entra em mistura
				}
				else
				{
					botao.Text = "(vazio)";
					botao.Disabled = true;
				}
			}
		}

		private void AoClicarItem(int indice)
		{
			int marcados = _botoesItem.Count(b => b != null && b.ButtonPressed);
			if (marcados > 2)
			{
				_botoesItem[indice].ButtonPressed = false;
				Mensagem("Só dá para misturar 2 ingredientes de cada vez.");
			}
		}

		private void Misturar()
		{
			var itens = _gameManager.Inventario;
			var escolhidos = Enumerable.Range(0, 3)
				.Where(i => i < itens.Count && _botoesItem[i] != null && _botoesItem[i].ButtonPressed)
				.Select(i => itens[i])
				.ToList();

			if (escolhidos.Count != 2)
			{
				Mensagem("Escolha exatamente 2 ingredientes para misturar.");
				return;
			}

			if (itens.Any(RegrasPuzzleEnfermaria.EhRemedio))
			{
				Mensagem("Você já está carregando um remédio. Leve-o a um paciente antes de preparar outro.");
				return;
			}

			string a = escolhidos[0];
			string b = escolhidos[1];
			bool aEspecifica = RegrasPuzzleEnfermaria.EhEspecifica(a);
			bool bEspecifica = RegrasPuzzleEnfermaria.EhEspecifica(b);

			if (aEspecifica == bEspecifica)
			{
				// Mistura sem sentido: efeito engraçado, e os ingredientes continuam com o jogador.
				Mensagem(RegrasPuzzleEnfermaria.MensagemMisturaInvalida(aEspecifica));
				EfeitoFalha();
				return;
			}

			string especifica = aEspecifica ? a : b;
			string sintoma = aEspecifica ? b : a;

			_gameManager.RemoverItem(a);
			_gameManager.RemoverItem(b);
			string remedio = RegrasPuzzleEnfermaria.CriarRemedio(especifica, sintoma);
			_gameManager.AdicionarItem(remedio);

			Mensagem("Misturado! O remédio está pronto. Leve até o paciente que precisa dele.");
			EfeitoSucesso(RegrasPuzzleEnfermaria.CorDoItem(remedio));
			AtualizarBotoes();
		}

		private void ParaEfeito()
		{
			if (_efeito != null && _efeito.IsValid()) _efeito.Kill();
			_efeito = null;

			if (_bequer == null) return;
			_bequer.Position = _posicaoOriginal;
			_bequer.Scale = _escalaOriginal;
			_bequer.Modulate = Colors.White;
		}

		private void EfeitoSucesso(Color cor)
		{
			if (_bequer == null) return;
			ParaEfeito();

			_bequer.PivotOffset = _bequer.Size / 2f;
			_efeito = CreateTween();
			_efeito.TweenProperty(_bequer, "modulate", cor, 0.25);
			_efeito.TweenProperty(_bequer, "scale", _escalaOriginal * 1.08f, 0.15);
			_efeito.TweenProperty(_bequer, "scale", _escalaOriginal, 0.15);
			_efeito.TweenProperty(_bequer, "modulate", Colors.White, 0.6);
		}

		private void EfeitoFalha()
		{
			if (_bequer == null) return;
			ParaEfeito();

			_fumaca?.Restart();
			_efeito = CreateTween();
			_efeito.TweenProperty(_bequer, "modulate", new Color(0.55f, 0.70f, 0.25f), 0.15); // verde-sujo
			for (int i = 0; i < 3; i++)
			{
				_efeito.TweenProperty(_bequer, "position:x", _posicaoOriginal.X + 2f, 0.04);
				_efeito.TweenProperty(_bequer, "position:x", _posicaoOriginal.X - 2f, 0.04);
			}
			_efeito.TweenProperty(_bequer, "position:x", _posicaoOriginal.X, 0.04);
			_efeito.TweenProperty(_bequer, "modulate", Colors.White, 0.6);
		}
	}
}
