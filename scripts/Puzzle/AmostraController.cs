using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.Puzzle
{
    // Controla uma amostra no Laboratório: recebe o método escolhido pelo jogador
    // e mostra a reação correspondente.
    public partial class AmostraController : Node2D
    {
        [Export] public string NomeMicrorganismo;
        [Export] public MetodoLaboratorio MetodoCorreto;

        private Amostra _amostra;
        private ReacaoService _reacaoService = new ReacaoService();

        public override void _Ready()
        {
            _amostra = new Amostra
            {
                Id = Name,
                NomeMicrorganismo = NomeMicrorganismo,
                MetodoCorreto = MetodoCorreto
            };
        }

        public void TestarMetodo(MetodoLaboratorio metodo)
        {
            _amostra.foiTestada = true;

            var reacao = _reacaoService.AplicarMetodo(_amostra, metodo);
            GD.Print("testando: " + metodo);

            // quem resolve a amostra é o método correto, a reação é só o efeito visual
            if (metodo == _amostra.MetodoCorreto)
            {
                _amostra.Resolvida = true;
            }

            MostrarReacaoVisual(reacao);
        }

        public bool EstaResolvida() => _amostra.Resolvida;

        private void MostrarReacaoVisual(TipoReacao reacao)
        {
            // TODO: trocar por animação/partícula de verdade quando a arte estiver pronta
            switch (reacao)
            {
                case TipoReacao.Evaporou:
                    Modulate = new Color(1, 1, 1, 0);
                    break;
                case TipoReacao.MudouCor:
                    Modulate = new Color(0.4f, 0.8f, 0.4f);
                    break;
                case TipoReacao.Multiplicou:
                    Scale *= 1.3f;
                    break;
                case TipoReacao.FezEspuma:
                    Modulate = new Color(0.6f, 1f, 0.6f);
                break;
            }
        }
    }
}