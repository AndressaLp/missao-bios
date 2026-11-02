using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.Puzzle.Laboratorio
{
    public enum QualidadeResposta { Correta, Parcial, Errada }

    public class OpcaoMetodo
    {
        public string Metodo;
        public QualidadeResposta Qualidade;
    }

    public static class RegrasPuzzleLaboratorio
    {
        public const float PontosCorreta = 11f;
        public const float PontosParcial = 5f;
        public const float PontosErrada = -5f;
        public const int MaxDicas = 3;
        public const float CustoPorDica = 2f;

        public static float Pontos(QualidadeResposta q) => q switch
        {
            QualidadeResposta.Correta => PontosCorreta,
            QualidadeResposta.Parcial => PontosParcial,
            _ => PontosErrada,
        };

        public static float PontosFinais(QualidadeResposta q, int dicasUsadas)
        {
            float pontos = Pontos(q);
            if (pontos <= 0f) return pontos;
            return Math.Max(0f, pontos - dicasUsadas * CustoPorDica);
        }

        public static List<OpcaoMetodo> GerarOpcoes(Microrganismo m)
        {
            string correto = m.MetodoCorreto;
            string parcial;
            List<string> errados;

            switch (m.Tipo)
            {
                case "bacteria":
                    parcial = correto == "calor" ? "quimico" : "calor";
                    errados = new List<string> { "uv", "filtragem" };
                    break;
                case "virus":
                    parcial = correto == "uv" ? "quimico" : "uv";
                    errados = new List<string> { "calor", "filtragem" };
                    break;
                default: // protozoario
                    parcial = "calor";
                    errados = new List<string> { "uv", "quimico" };
                    break;
            }

            var rnd = new Random();
            var opcoes = new List<OpcaoMetodo>
            {
                new OpcaoMetodo { Metodo = correto, Qualidade = QualidadeResposta.Correta },
                new OpcaoMetodo { Metodo = parcial, Qualidade = QualidadeResposta.Parcial },
                new OpcaoMetodo { Metodo = errados[rnd.Next(errados.Count)], Qualidade = QualidadeResposta.Errada },
            };

            for (int i = opcoes.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (opcoes[i], opcoes[j]) = (opcoes[j], opcoes[i]);
            }
            return opcoes;
        }

        public static string NomeAmigavel(string metodo) => metodo switch
        {
            "calor" => "Aquecer a amostra",
            "uv" => "Aplicar luz ultravioleta",
            "quimico" => "Usar substância química",
            "filtragem" => "Filtrar a amostra",
            _ => metodo,
        };

        public static string TextoDica(Microrganismo m, int numero) => numero switch
        {
            1 => m.Tipo switch
            {
                "bacteria" => "É uma bactéria: um ser vivo de uma só célula, bem pequenininho.",
                "virus" => "É um vírus: ainda menor que uma bactéria! Só se multiplica dentro de outras células.",
                _ => "É um protozoário: tem uma só célula, mas é bem maior que bactérias e vírus.",
            },
            2 => m.PistaMetodo,
            _ => "Descartei uma opção que não é a melhor aqui.",
        };

        private static string FraseDoMetodo(string metodo) => metodo switch
        {
            "calor" => "O calor destrói as células de muitos microrganismos. É por isso que fervemos a água!",
            "uv" => "A luz ultravioleta danifica o material genético dos microrganismos e impede que eles se multipliquem.",
            "quimico" => "Desinfetantes, como o cloro, quebram a proteção externa dos microrganismos.",
            "filtragem" => "Filtros prendem seres grandes, mas os bem pequenininhos passam pelos furinhos.",
            _ => "",
        };

        public static string Explicacao(Microrganismo m, QualidadeResposta q)
        {
            string melhor = NomeAmigavel(m.MetodoCorreto);
            return q switch
            {
                QualidadeResposta.Correta =>
                    $"Isso mesmo! {m.PistaMetodo} {FraseDoMetodo(m.MetodoCorreto)}",
                QualidadeResposta.Parcial =>
                    $"Quase! Esse método ajuda, mas o melhor aqui é: {melhor}. {m.PistaMetodo}",
                _ =>
                    $"Esse método não é o mais indicado aqui. O melhor é: {melhor}. {m.PistaMetodo} {FraseDoMetodo(m.MetodoCorreto)}",
            };
        }
    }
}