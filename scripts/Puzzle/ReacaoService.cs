using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.Puzzle
{
    public enum TipoReacao
    {
        Evaporou,
        MudouCor,
        Multiplicou,
        FezEspuma,
        SemReacao
    }

    // Decide o que acontece quando um método é aplicado numa amostra.
    // TODO: substituir esses dados de teste pelos micro-organismos definitivos.
    public class ReacaoService
    {
        public TipoReacao AplicarMetodo(Amostra amostra, MetodoLaboratorio metodo)
        {
            if (amostra.NomeMicrorganismo == "Bacilo Termal")
            {
                if (metodo == MetodoLaboratorio.Calor) return TipoReacao.Evaporou;
                if (metodo == MetodoLaboratorio.Frio) return TipoReacao.MudouCor;
                if (metodo == MetodoLaboratorio.Luz) return TipoReacao.SemReacao;
                if (metodo == MetodoLaboratorio.Misturar) return TipoReacao.FezEspuma;
            }

            if (amostra.NomeMicrorganismo == "Protoz Aquatico")
            {
                if (metodo == MetodoLaboratorio.Calor) return TipoReacao.Multiplicou;
                if (metodo == MetodoLaboratorio.Frio) return TipoReacao.Evaporou;
                if (metodo == MetodoLaboratorio.Luz) return TipoReacao.SemReacao;
                if (metodo == MetodoLaboratorio.Misturar) return TipoReacao.MudouCor;
            }

            if (amostra.NomeMicrorganismo == "Viron Solar")
            {
                if (metodo == MetodoLaboratorio.Calor) return TipoReacao.SemReacao;
                if (metodo == MetodoLaboratorio.Frio) return TipoReacao.SemReacao;
                if (metodo == MetodoLaboratorio.Luz) return TipoReacao.Evaporou;
                if (metodo == MetodoLaboratorio.Misturar) return TipoReacao.Multiplicou;
            }

            return TipoReacao.SemReacao;
        }
    }
}