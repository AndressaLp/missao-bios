using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MissaoBios.scripts.Managers;
using MissaoBios.scripts.Puzzle.Laboratorio;

namespace MissaoBios.scripts.UI
{
    public partial class MicroscopioUI : CanvasLayer
    {
        private const string PastaDesenhos = "res://assets/art/microrganismos/";
        private static readonly string[] NomesAmostras = { "Amostra A", "Amostra B", "Amostra C" };

        private CanvasItem _telaSelecao;
        private readonly Button[] _botoesAmostra = new Button[3];
        private Button _botaoFechar;

        private CanvasItem _telaDescricao;
        private Label _labelNome;
        private Label _labelDescricao;
        private TextureRect _imagem;
        private Button _botaoVoltar;
        private Button _botaoDica;
        private CanvasItem _painelKael;
        private Label _labelKael;
        private readonly Button[] _botoesOpcao = new Button[3];

        private PuzzleManager _puzzleManager;
        private GameManager _gameManager;

        private int _amostraAtual = -1;
        private bool _respondida;
        private List<OpcaoMetodo> _opcoesAtuais = new();

        public override void _Ready()
        {
            Layer = 16;
            ProcessMode = ProcessModeEnum.Always;
            Visible = false;

            _telaSelecao = ObterNo<CanvasItem>("Painel/TelaSelecao");
            _telaDescricao = ObterNo<CanvasItem>("Painel/TelaDescricao");

            _botaoFechar = ObterNo<Button>("Painel/TelaSelecao/BotaoFechar");
            if (_botaoFechar != null) _botaoFechar.Pressed += Fechar;

            _botaoVoltar = ObterNo<Button>("Painel/TelaDescricao/BotaoVoltar");
            if (_botaoVoltar != null) _botaoVoltar.Pressed += VoltarParaSelecao;

            _botaoDica = ObterNo<Button>("Painel/TelaDescricao/BotaoDica");
            if (_botaoDica != null) _botaoDica.Pressed += PedirDica;

            for (int i = 0; i < 3; i++)
            {
                int indice = i;
                _botoesAmostra[i] = ObterNo<Button>($"Painel/TelaSelecao/BotaoAmostra{i + 1}");
                if (_botoesAmostra[i] != null) _botoesAmostra[i].Pressed += () => AbrirAmostra(indice);

                _botoesOpcao[i] = ObterNo<Button>($"Painel/TelaDescricao/BotaoOpcao{i + 1}");
                if (_botoesOpcao[i] != null) _botoesOpcao[i].Pressed += () => EscolherOpcao(indice);
            }

            _labelNome = ObterNo<Label>("Painel/TelaDescricao/PainelInfo/LabelNome");
            _labelDescricao = ObterNo<Label>("Painel/TelaDescricao/PainelInfo/LabelDescricao");
            _imagem = ObterNo<TextureRect>("Painel/TelaDescricao/FundoLente/ImagemMicrorganismo");
            _painelKael = ObterNo<CanvasItem>("Painel/TelaDescricao/PainelKael");
            _labelKael = ObterNo<Label>("Painel/TelaDescricao/PainelKael/LabelKael");

            GetNodeOrNull<CanvasItem>("Painel/TelaDescricao/LabelResultado")?.Hide();

            _puzzleManager = GetNodeOrNull<PuzzleManager>("/root/PuzzleManager");
            _gameManager = GetNodeOrNull<GameManager>("/root/GameManager");

            IgnorarCliquesDecorativos(this); // painéis e imagens não "roubam" o clique dos botões
        }

        private T ObterNo<T>(string caminho) where T : Node
        {
            var no = GetNodeOrNull<T>(caminho);
            if (no == null) GD.PrintErr($"[MicroscopioUI] Nó não encontrado: '{caminho}'");
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

            if (_telaDescricao.Visible) VoltarParaSelecao();
            else Fechar();
            GetViewport().SetInputAsHandled();
        }

        public void Abrir()
        {
            _puzzleManager.GarantirRodadaIniciada();
            if (_puzzleManager.AmostrasAtuais.Count < 3)
            {
                GD.PrintErr("[Microscópio] Sem amostras sorteadas (confira o PuzzleManager e o JSON).");
                return;
            }

            GetNodeOrNull<CanvasItem>("Painel")?.Show();
            MostrarSelecao();
            Visible = true;
            GetTree().Paused = true;
        }

        private void MostrarSelecao()
        {
            _amostraAtual = -1;
            _telaDescricao.Visible = false;
            _telaSelecao.Visible = true;

            for (int i = 0; i < 3; i++)
            {
                var amostra = _puzzleManager.AmostrasAtuais[i];
                _botoesAmostra[i].Text = amostra.Resolvida ? $"{NomesAmostras[i]} (feita)" : NomesAmostras[i];
                _botoesAmostra[i].Disabled = amostra.Resolvida;
            }
        }

        private void AbrirAmostra(int indice)
        {
            _amostraAtual = indice;
            _respondida = false;
            var dados = _puzzleManager.AmostrasAtuais[indice].Dados;

            _labelNome.Text = NomesAmostras[indice];
            _labelDescricao.Text = dados.Comportamento;

            string caminho = $"{PastaDesenhos}{dados.Id}.png";
            var textura = ResourceLoader.Exists(caminho) ? GD.Load<Texture2D>(caminho) : null;
            _imagem.Texture = textura;
            _imagem.Visible = textura != null;

            _opcoesAtuais = RegrasPuzzleLaboratorio.GerarOpcoes(dados);
            for (int i = 0; i < 3; i++)
            {
                _botoesOpcao[i].Text = RegrasPuzzleLaboratorio.NomeAmigavel(_opcoesAtuais[i].Metodo);
                _botoesOpcao[i].Disabled = false;
            }

            AtualizarDicas();

            _telaSelecao.Visible = false;
            _telaDescricao.Visible = true;
        }

        private void PedirDica()
        {
            if (_respondida || _amostraAtual < 0) return;

            var amostra = _puzzleManager.AmostrasAtuais[_amostraAtual];
            if (amostra.DicasUsadas >= RegrasPuzzleLaboratorio.MaxDicas) return;

            amostra.DicasUsadas++;
            AtualizarDicas();
        }

        private void AtualizarDicas()
        {
            var amostra = _puzzleManager.AmostrasAtuais[_amostraAtual];
            int usadas = amostra.DicasUsadas;

            if (usadas == 0)
            {
                if (_painelKael != null) _painelKael.Visible = false;
            }
            else
            {
                var linhas = new List<string>();
                for (int n = 1; n <= usadas; n++)
                    linhas.Add($"Dica {n} (-{RegrasPuzzleLaboratorio.CustoPorDica:0} pts): {RegrasPuzzleLaboratorio.TextoDica(amostra.Dados, n)}");
                MostrarKael(string.Join("\n", linhas));
            }

            if (usadas >= RegrasPuzzleLaboratorio.MaxDicas)
            {
                int idx = _opcoesAtuais.FindIndex(o => o.Qualidade == QualidadeResposta.Errada);
                if (idx >= 0)
                {
                    _botoesOpcao[idx].Disabled = true;
                    _botoesOpcao[idx].Text = "(descartada)";
                }
            }

            int restantes = RegrasPuzzleLaboratorio.MaxDicas - usadas;
            _botaoDica.Text = $"Dica ({restantes})";
            _botaoDica.Disabled = restantes <= 0 || _respondida;
        }

        private void MostrarKael(string texto)
        {
            if (_painelKael != null) _painelKael.Visible = true;
            if (_labelKael != null) _labelKael.Text = "Dra. Kael: " + texto;
        }

        private void EscolherOpcao(int indiceOpcao)
        {
            if (_respondida || _amostraAtual < 0) return;
            _respondida = true;

            var amostra = _puzzleManager.AmostrasAtuais[_amostraAtual];
            var opcao = _opcoesAtuais[indiceOpcao];
            amostra.Resolvida = true;
            amostra.Resposta = opcao.Qualidade;
            _gameManager.RegistrarDescoberta(amostra.Dados.Id, true, false); // a explicação revela o melhor método

            float pontos = RegrasPuzzleLaboratorio.PontosFinais(opcao.Qualidade, amostra.DicasUsadas);
            _gameManager.AlterarPontuacao(pontos, "puzzle1");

            string veredito = opcao.Qualidade switch
            {
                QualidadeResposta.Correta => "Correto!",
                QualidadeResposta.Parcial => "Quase!",
                _ => "Não foi dessa vez.",
            };
            string sinal = pontos >= 0 ? "+" : "";
            bool descontou = RegrasPuzzleLaboratorio.Pontos(opcao.Qualidade) > 0 && amostra.DicasUsadas > 0;
            string extra = descontou
                ? $" (dicas: -{amostra.DicasUsadas * RegrasPuzzleLaboratorio.CustoPorDica:0})"
                : "";

            _labelNome.Text = amostra.Dados.Nome;
            _labelDescricao.Text = $"{veredito} {sinal}{pontos:0} pontos{extra}";
            MostrarKael(RegrasPuzzleLaboratorio.Explicacao(amostra.Dados, opcao.Qualidade));

            foreach (var b in _botoesOpcao) b.Disabled = true;
            _botaoDica.Disabled = true;

            int feitas = _puzzleManager.AmostrasAtuais.Count(a => a.Resolvida);
            _gameManager.DefinirTextoMissao("analisar_amostras", $"Analise as amostras no microscópio ({feitas}/3)");
            if (feitas >= 3) _gameManager.ConcluirMissao("analisar_amostras");
            GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
        }

        private void VoltarParaSelecao() => MostrarSelecao();

        private void Fechar()
        {
            Visible = false;
            GetTree().Paused = false;
        }
    }
}