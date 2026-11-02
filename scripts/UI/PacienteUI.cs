using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MissaoBios.scripts.Managers;
using MissaoBios.scripts.Puzzle.Laboratorio;
using MissaoBios.scripts.Puzzle.Enfermaria;

namespace MissaoBios.scripts.UI
{
    public partial class PacienteUI : CanvasLayer
    {
        private const string PastaArte = "res://assets/art/pacientes/";

        private Label _labelNome;
        private Label _labelFicha;
        private TextureRect _imagem;
        private TextureRect _manchas;
        private Button _botaoVoltar;
        private Button _botaoDica;
        private readonly CanvasItem[] _paineisDica = new CanvasItem[3];
        private readonly Label[] _labelsDica = new Label[3];

        private PuzzleManager _puzzleManager;
        private GameManager _gameManager;
        private PacienteEmJogo _paciente;
        private Tween _tremor;
        private Vector2 _posicaoOriginal;

        private Button _botaoRemedio;
        private Tween _reacao;
        private string _mensagemJack = "";

        public override void _Ready()
        {
            Layer = 16;
            ProcessMode = ProcessModeEnum.Always; // funciona com o jogo pausado
            Visible = false;

            _labelNome = ObterNo<Label>("Control/PainelInfo/LabelNome");
            _labelFicha = ObterNo<Label>("Control/PainelInfo/LabelFicha");
            _imagem = ObterNo<TextureRect>("Control/ImagemPaciente");
            _manchas = GetNodeOrNull<TextureRect>("Control/ImagemPaciente/ManchasPaciente"); // opcional
            for (int i = 0; i < 3; i++)
            {
                _paineisDica[i] = ObterNo<CanvasItem>($"Control/PainelDica{i + 1}");
                _labelsDica[i] = ObterNo<Label>($"Control/PainelDica{i + 1}/LabelDica");
            }

            _botaoVoltar = ObterNo<Button>("Control/BotaoVoltar");
            if (_botaoVoltar != null) _botaoVoltar.Pressed += Fechar;

            _botaoDica = ObterNo<Button>("Control/BotaoDica");
            if (_botaoDica != null) _botaoDica.Pressed += PedirDica;

            _botaoRemedio = ObterNo<Button>("Control/BotaoRemedio");
            if (_botaoRemedio != null) _botaoRemedio.Pressed += DarRemedio;

            if (_imagem != null) _posicaoOriginal = _imagem.Position;

            _puzzleManager = GetNodeOrNull<PuzzleManager>("/root/PuzzleManager");
            _gameManager = GetNodeOrNull<GameManager>("/root/GameManager");

            IgnorarCliquesDecorativos(this);
        }

        private T ObterNo<T>(string caminho) where T : Node
        {
            var no = GetNodeOrNull<T>(caminho);
            if (no == null) GD.PrintErr($"[PacienteUI] Nó não encontrado: '{caminho}'");
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

        public void Abrir(int posicao)
        {
            if (!_puzzleManager.GarantirRodadaEnfermariaIniciada()) return;
            if (posicao < 1 || posicao > _puzzleManager.PacientesAtuais.Count) return;

            _paciente = _puzzleManager.PacientesAtuais[posicao - 1];
            _mensagemJack = "";
            MostrarFicha();
            AtualizarDicas();
            AtualizarBotaoRemedio();

            Visible = true;
            GetTree().Paused = true;

            if (!_paciente.Examinado)
            {
                _paciente.Examinado = true;
                int examinados = _puzzleManager.PacientesAtuais.Count(p => p.Examinado);
                _gameManager.DefinirTextoMissao("examinar_pacientes", $"Examine os pacientes ({examinados}/3)");
                if (examinados >= 3) _gameManager.ConcluirMissao("examinar_pacientes");
                GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
            }
        }

        private void MostrarFicha()
        {
            var m = _paciente.Dados;
            var s = m.SintomaPaciente;

            _labelNome.Text = RegrasPuzzleEnfermaria.NomesPacientes[_paciente.Posicao - 1];

            if (_paciente.Curado)
            {
                _labelFicha.Text = "Já foi curado e voltou ao trabalho!";
                MostrarCurado();
                return;
            }

            var linhas = new List<string>
            {
                $"Temperatura: {Formatar(s.TemperaturaC)} °C",
                $"Batimentos: {s.FrequenciaCardiacaBpm} por minuto",
                $"Sintoma: {s.SintomaExtra}",
            };
            if (s.ManchaVisual != "nenhuma") linhas.Add($"Manchas {s.ManchaVisual}s na pele");
            linhas.Add($"Exame: {m.Nome} ({RegrasPuzzleEnfermaria.TipoAmigavel(m.Tipo)})");
            _labelFicha.Text = string.Join("\n", linhas);

            AplicarVisual(s);
        }

        // 38,5 em vez de 38.5, independente do idioma do computador.
        private static string Formatar(float valor) => valor.ToString("0.0").Replace('.', ',');

        private void AplicarVisual(SintomaPaciente s)
        {
            PararTremor();

            string caminho = $"{PastaArte}paciente{_paciente.Posicao}.png";
            var textura = ResourceLoader.Exists(caminho) ? GD.Load<Texture2D>(caminho) : null;
            _imagem.Texture = textura;
            _imagem.Visible = textura != null;
            _imagem.Modulate = s.TemperaturaC >= 38f ? new Color(1f, 0.8f, 0.8f) : Colors.White; // febre = avermelhado

            if (_manchas != null)
            {
                string arteManchas = $"{PastaArte}manchas.png";
                bool temManchas = s.ManchaVisual != "nenhuma" && ResourceLoader.Exists(arteManchas);
                _manchas.Visible = temManchas;
                if (temManchas)
                {
                    _manchas.Texture = GD.Load<Texture2D>(arteManchas);
                    _manchas.Modulate = s.ManchaVisual switch
                    {
                        "roxa" => new Color(0.6f, 0.3f, 0.8f),
                        "avermelhada" => new Color(0.9f, 0.3f, 0.3f),
                        _ => new Color(0.9f, 0.85f, 0.3f),
                    };
                }
            }

            // Coração acelerado = paciente tremendo
            if (_imagem.Visible && s.FrequenciaCardiacaBpm >= 110)
            {
                _tremor = CreateTween().SetLoops();
                _tremor.TweenProperty(_imagem, "position:x", _posicaoOriginal.X + 1.5f, 0.05);
                _tremor.TweenProperty(_imagem, "position:x", _posicaoOriginal.X - 1.5f, 0.05);
            }
        }

        private void PararTremor()
        {
            if (_tremor != null && _tremor.IsValid()) _tremor.Kill();
            _tremor = null;
            if (_imagem != null) _imagem.Position = _posicaoOriginal;
        }

        private void MostrarCurado()
        {
            PararTremor();

            string caminho = $"{PastaArte}paciente{_paciente.Posicao}.png";
            var textura = ResourceLoader.Exists(caminho) ? GD.Load<Texture2D>(caminho) : null;
            _imagem.Texture = textura;
            _imagem.Visible = textura != null;
            _imagem.Modulate = Colors.White;
            if (_manchas != null) _manchas.Visible = false;
        }

        private void AtualizarBotaoRemedio()
        {
            if (_botaoRemedio == null) return;

            bool temRemedio = _gameManager.Inventario.Any(RegrasPuzzleEnfermaria.EhRemedio);
            _botaoRemedio.Text = temRemedio ? "Dar remédio" : "Sem remédio";
            _botaoRemedio.Disabled = !temRemedio || _paciente.Curado;
        }

        private void DarRemedio()
        {
            if (_paciente == null || _paciente.Curado) return;

            string remedio = _gameManager.Inventario.FirstOrDefault(RegrasPuzzleEnfermaria.EhRemedio);
            if (remedio == null) return;

            _gameManager.RemoverItem(remedio); // o remédio é gasto em qualquer tentativa
            _paciente.Tentativas++;

            var resultado = RegrasPuzzleEnfermaria.AvaliarRemedio(_paciente.Dados, remedio);
            string nome = RegrasPuzzleEnfermaria.NomesPacientes[_paciente.Posicao - 1];
            _mensagemJack = RegrasPuzzleEnfermaria.MensagemResultado(resultado, nome);

            if (resultado == ResultadoRemedio.Correto)
            {
                int falhas = _paciente.Tentativas - 1;
                float pontos = RegrasPuzzleEnfermaria.PontosDaCura(falhas, _paciente.DicasUsadas);
                _gameManager.AlterarPontuacao(pontos, "puzzle2");
                _paciente.Curado = true;
                _gameManager.RegistrarDescoberta(_paciente.Dados.Id, false, true);
                _mensagemJack += $" (+{pontos:0} pontos)";

                int curados = _puzzleManager.PacientesAtuais.Count(p => p.Curado);
                _gameManager.DefinirTextoMissao("curar_pacientes", $"Cure os pacientes ({curados}/3)");
                if (curados >= 3) _gameManager.ConcluirMissao("curar_pacientes");

                _labelFicha.Text = "Já foi curado e voltou ao trabalho!";
                MostrarCurado();
            }

            ReacaoVisual(resultado);
            AtualizarDicas();
            AtualizarBotaoRemedio();
            GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
        }

        private void ReacaoVisual(ResultadoRemedio resultado)
        {
            if (_imagem == null || !_imagem.Visible) return;

            PararTremor();
            if (_reacao != null && _reacao.IsValid()) _reacao.Kill();

            _reacao = CreateTween();
            switch (resultado)
            {
                case ResultadoRemedio.Correto:
                    _imagem.Modulate = new Color(0.7f, 1f, 0.7f);
                    _reacao.TweenProperty(_imagem, "modulate", Colors.White, 0.8);
                    break;

                case ResultadoRemedio.Errado: // paciente fica verde e flutua
                    _imagem.Modulate = new Color(0.6f, 0.9f, 0.5f);
                    for (int i = 0; i < 3; i++)
                    {
                        _reacao.TweenProperty(_imagem, "position:y", _posicaoOriginal.Y - 6f, 0.25);
                        _reacao.TweenProperty(_imagem, "position:y", _posicaoOriginal.Y, 0.25);
                    }
                    break;

                default: // só uma parte certa
                    _imagem.Modulate = new Color(1f, 0.95f, 0.6f);
                    _reacao.TweenProperty(_imagem, "modulate", Colors.White, 0.8);
                    break;
            }

            if (resultado != ResultadoRemedio.Correto)
                _reacao.Finished += VoltarAoVisualDoPaciente;
        }

        private void VoltarAoVisualDoPaciente()
        {
            if (Visible && _paciente != null && !_paciente.Curado)
                AplicarVisual(_paciente.Dados.SintomaPaciente);
        }

        private void PedirDica()
        {
            if (_paciente == null || _paciente.Curado) return;

            // Depois de um remédio, o 1º clique só volta a mostrar as dicas já usadas.
            if (!string.IsNullOrEmpty(_mensagemJack) && _paciente.DicasUsadas > 0)
            {
                _mensagemJack = "";
                AtualizarDicas();
                return;
            }

            if (_paciente.DicasUsadas >= RegrasPuzzleEnfermaria.MaxDicas) return;

            _mensagemJack = "";
            _paciente.DicasUsadas++;
            AtualizarDicas();
        }

        private void AtualizarDicas()
        {
            int usadas = _paciente.DicasUsadas;
            var m = _paciente.Dados;
            bool temMensagem = !string.IsNullOrEmpty(_mensagemJack);

            Substancia especifica = null, sintoma = null;
            if (usadas > 0)
            {
                especifica = _puzzleManager.ObterSubstancia(m.CuraIngredientes[0]);
                sintoma = _puzzleManager.ObterSubstancia(m.CuraIngredientes[1]);
            }

            for (int i = 0; i < 3; i++)
            {
                if (_paineisDica[i] == null || _labelsDica[i] == null) continue;

                bool mostrar;
                string texto = "";
                if (temMensagem)
                {
                    mostrar = i == 0; // o resultado do remédio ocupa o 1º painel
                    if (mostrar) texto = "Dr. Jack: " + _mensagemJack;
                }
                else
                {
                    mostrar = i < usadas && !_paciente.Curado;
                    if (mostrar)
                        texto = $"Dica {i + 1} (-{RegrasPuzzleEnfermaria.CustoPorDica:0} pts): {RegrasPuzzleEnfermaria.TextoDica(m, especifica, sintoma, i + 1)}";
                }

                _paineisDica[i].Visible = mostrar;
                _labelsDica[i].Text = texto;
            }

            int restantes = RegrasPuzzleEnfermaria.MaxDicas - usadas;
            bool voltarParaDicas = temMensagem && usadas > 0;
            _botaoDica.Text = voltarParaDicas ? "Ver dicas" : $"Dica ({restantes})";
            _botaoDica.Disabled = _paciente.Curado || (!voltarParaDicas && restantes <= 0);
        }

        private void Fechar()
        {
            if (_reacao != null && _reacao.IsValid()) _reacao.Kill();
            _reacao = null;
            PararTremor();
            Visible = false;
            GetTree().Paused = false;
        }
    }
}