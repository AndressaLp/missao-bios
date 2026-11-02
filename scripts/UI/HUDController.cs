using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;
using MissaoBios.scripts.Puzzle.Enfermaria;

namespace MissaoBios.scripts.UI
{
    public partial class HUDController : CanvasLayer
    {
        private const double DuracaoVerde = 3.0;
        private Label _labelNomeJogador;
        private RichTextLabel _labelMissoes;
        private Control _painelMissoes;
        private Button _botaoSom;
        private AudioManager _audioManager;
        private GameManager _gameManager;
        private Godot.Timer _timerAtualizar;
        private Control _inventario;
        private DialogueManager _dialogueManager;
        private readonly Control[] _slots = new Control[3];
        private PuzzleManager _puzzleManager;
        [Export] public Texture2D IconeFrasco; // arte do frasco é pintada com a cor de cada ingrediente
        private Label _nomeSlot;

        public override void _Ready()
        {
            _labelNomeJogador = ObterNo<Label>("LabelNomeJogador");
            _labelMissoes = ObterNo<RichTextLabel>("PainelMissoes/ScrollContainer/LabelMissoes");
            _painelMissoes = ObterNo<Control>("PainelMissoes");
            _audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
            _gameManager = GetNodeOrNull<GameManager>("/root/GameManager");
            _puzzleManager = GetNodeOrNull<PuzzleManager>("/root/PuzzleManager");
            ConfigurarSlots();
            if (_gameManager != null) _gameManager.InventarioAtualizado += AtualizarInventario;
            AtualizarInventario();
            _inventario = ObterNo<Control>("InventarioContainer");
            _dialogueManager = GetNodeOrNull<DialogueManager>("/root/DialogueManager");
            if (_dialogueManager != null)
            {
                _dialogueManager.FalaAtualizada += AoFalaAtualizada;
                _dialogueManager.DialogoEncerrado += AoDialogoEncerrado;
                if (_inventario != null) _inventario.Visible = !_dialogueManager.EmDialogo;
            }

            if (_labelMissoes != null)
            {
                _labelMissoes.BbcodeEnabled = true;
                _labelMissoes.ScrollActive = false;
            }

            ConectarBotao("BotoesSecundarios/BotaoPausa", AbrirPausa);

            _botaoSom = ObterNo<Button>("BotoesSecundarios/BotaoSom");
            if (_botaoSom != null) _botaoSom.Pressed += AlternarSom;

            var saveManager = GetNodeOrNull<SaveManager>("/root/SaveManager");
            if (saveManager?.PerfilAtivo != null && _labelNomeJogador != null)
                _labelNomeJogador.Text = saveManager.PerfilAtivo.Nome;

            _timerAtualizar = new Godot.Timer { OneShot = true };
            AddChild(_timerAtualizar);
            _timerAtualizar.Timeout += AtualizarMissoes;

            if (_gameManager != null) _gameManager.MissoesAtualizadas += AtualizarMissoes;
            AtualizarMissoes();
        }

        public override void _ExitTree()
        {
            // O GameManager continua vivo entre salas; sem isso ele chamaria um HUD já destruído.
            if (_gameManager != null)
            _gameManager.MissoesAtualizadas -= AtualizarMissoes;
            
            if (_gameManager != null)
            {
                _dialogueManager.FalaAtualizada -= AoFalaAtualizada;
                _dialogueManager.DialogoEncerrado -= AoDialogoEncerrado;
            }

            if (_gameManager != null) _gameManager.InventarioAtualizado -= AtualizarInventario;
        }

        private T ObterNo<T>(string caminho) where T : Node
        {
            var no = GetNodeOrNull<T>(caminho);
            if (no == null)
                GD.PrintErr($"[HUD] Nó não encontrado: '{caminho}'");
            return no;
        }

        private void ConectarBotao(string caminho, Action acao)
        {
            var botao = ObterNo<Button>(caminho);
            if (botao != null) botao.Pressed += acao;
        }

        public void AtualizarMissoes()
        {
            if (_labelMissoes == null || _gameManager == null) return;

            double agora = Time.GetTicksMsec() / 1000.0;
            var linhas = new List<string>();
            double proximaExpiracao = double.MaxValue;

            foreach (var m in _gameManager.Missoes)
            {
                if (m.Estado == EstadoMissao.Ativa)
                {
                    linhas.Add(m.Texto);
                }
                else if (m.Estado == EstadoMissao.Concluida && m.ConcluidaEm > 0)
                {
                    double restante = DuracaoVerde - (agora - m.ConcluidaEm);
                    if (restante > 0)
                    {
                        linhas.Add($"[color=#5fd35f]{m.Texto}[/color]");
                        proximaExpiracao = Math.Min(proximaExpiracao, restante);
                    }
                }
            }
            if (_painelMissoes != null) _painelMissoes.Visible = linhas.Count > 0;
            _labelMissoes.Text = string.Join("\n", linhas);

            if (proximaExpiracao < double.MaxValue)
                _timerAtualizar.Start(proximaExpiracao + 0.05);
        }

        private void AoFalaAtualizada(string texto)
        {
            if (_inventario != null) _inventario.Visible = false;
            if (_nomeSlot != null) _nomeSlot.Visible = false;
        }

        private void AoDialogoEncerrado()
        {
            if (_inventario != null) _inventario.Visible = true;
        }

        private void AbrirPausa()
        {
            GetNode<PauseMenuController>("/root/PauseMenu").Abrir();
        }

        private void AlternarSom()
        {
            _audioManager.AlternarSom();
            _botaoSom.Text = _audioManager.SomAtivado ? "🔊" : "🔇";
        }

        private void ConfigurarSlots()
        {
            _nomeSlot = new Label { Visible = false, MouseFilter = Control.MouseFilterEnum.Ignore };
            _nomeSlot.AddThemeFontSizeOverride("font_size", 6);
            _nomeSlot.AddThemeColorOverride("font_outline_color", Colors.Black);
            _nomeSlot.AddThemeConstantOverride("outline_size", 3);
            AddChild(_nomeSlot);

            for (int i = 0; i < 3; i++)
            {
                int indice = i;
                var slot = GetNodeOrNull<Control>($"InventarioContainer/SlotItem{i + 1}");
                if (slot == null)
                {
                    GD.PrintErr($"[HUD] Nó não encontrado: 'InventarioContainer/SlotItem{i + 1}'");
                    continue;
                }

                Control icone;
                if (IconeFrasco != null)
                {
                    icone = new TextureRect
                    {
                        Texture = IconeFrasco,
                        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                        StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                    };
                }
                else
                {
                    icone = new ColorRect { Color = Colors.White }; // provisório, se o ícone não estiver ligado
                }

                icone.Name = "Icone";
                icone.Visible = false;
                icone.MouseFilter = Control.MouseFilterEnum.Ignore;
                icone.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect, Control.LayoutPresetMode.Minsize, 2);
                slot.AddChild(icone);

                slot.TooltipText = ""; // sem o tooltip padrão
                slot.GuiInput += ev =>
                {
                    if (ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
                        AoClicarSlot(indice);
                };
                slot.MouseEntered += () => MostrarNomeSlot(indice);
                slot.MouseExited += () => { if (_nomeSlot != null) _nomeSlot.Visible = false; };
                _slots[i] = slot;
            }
        }

        private void AtualizarInventario()
        {
            if (!IsInstanceValid(this) || _gameManager == null) return;
            if (_nomeSlot != null) _nomeSlot.Visible = false;

            for (int i = 0; i < 3; i++)
            {
                var icone = _slots[i]?.GetNodeOrNull<Control>("Icone");
                if (icone == null) continue;

                if (i < _gameManager.Inventario.Count)
                {
                    icone.Visible = true;
                    icone.Modulate = RegrasPuzzleEnfermaria.CorDoItem(_gameManager.Inventario[i]);
                }
                else
                {
                    icone.Visible = false;
                }
            }
        }

        private void MostrarNomeSlot(int indice)
        {
            if (_nomeSlot == null || _slots[indice] == null || _gameManager == null) return;
            if (indice >= _gameManager.Inventario.Count) { _nomeSlot.Visible = false; return; }

            string id = _gameManager.Inventario[indice];
            _nomeSlot.Text = RegrasPuzzleEnfermaria.EhRemedio(id) ? "Remédio" : RegrasPuzzleEnfermaria.NomeDoItem(_puzzleManager, id);
            _nomeSlot.ResetSize();

            var slot = _slots[indice];
            float largura = GetViewport().GetVisibleRect().Size.X;
            float x = Mathf.Clamp(slot.GlobalPosition.X + slot.Size.X / 2f - _nomeSlot.Size.X / 2f, 0f, largura - _nomeSlot.Size.X);
            _nomeSlot.Position = new Vector2(x, slot.GlobalPosition.Y - _nomeSlot.Size.Y - 2f);
            _nomeSlot.Visible = true;
        }

        private void AoClicarSlot(int indice)
        {
            var gm = _gameManager;
            var dm = GetNodeOrNull<DialogueManager>("/root/DialogueManager");
            if (gm == null || dm == null || dm.EmDialogo || indice >= gm.Inventario.Count) return;

            string id = gm.Inventario[indice];
            string info = RegrasPuzzleEnfermaria.InfoDoItem(_puzzleManager, id);
            if (string.IsNullOrEmpty(info)) return;

            bool remedio = RegrasPuzzleEnfermaria.EhRemedio(id);
            dm.IniciarFalasComEscolha(
                new[] { info, remedio ? "Quer descartar esse remédio?" : "Quer descartar esse ingrediente?" },
                "Descartar", "Guardar",
                descartar => { if (descartar) gm.RemoverItem(id); });
        }
    }
}