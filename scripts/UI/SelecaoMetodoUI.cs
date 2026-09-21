using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.UI
{
    // Botões de método (Calor, Frio, Luz, Misturar) que o jogador aperta durante o teste.
    public partial class SelecaoMetodoUI : Control
    {
        private AmostraController _amostraSelecionada;
        private int cliques = 0;

        public void SelecionarAmostra(AmostraController amostra)
        {
            _amostraSelecionada = amostra;
        }

        public void OnBotaoCalorPressed() => AplicarMetodo(MetodoLaboratorio.Calor);
        public void OnBotaoFrioPressed() => AplicarMetodo(MetodoLaboratorio.Frio);
        public void OnBotaoLuzPressed() => AplicarMetodo(MetodoLaboratorio.Luz);
        public void OnBotaoMisturarPressed() => AplicarMetodo(MetodoLaboratorio.Misturar);

        private void AplicarMetodo(MetodoLaboratorio metodo)
        {
            if (_amostraSelecionada == null) return;

            cliques = cliques + 1;
            _amostraSelecionada.TestarMetodo(metodo);
        }
    }
}