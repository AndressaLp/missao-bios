using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissaoBios.scripts.Puzzle.Laboratorio;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.Puzzle.Enfermaria
{
    public enum ResultadoRemedio { Correto, SoEspecificaCerta, SoSintomaCerto, Errado }

    public static class RegrasPuzzleEnfermaria
    {
        public const string PrefixoRemedio = "remedio:";
        public const float PontosCura = 11f;
        public const float PenalidadePorFalha = 3f;
        public const float MinimoCura = 2f;

        public static Color CorDaSubstancia(string id) => id switch
        {
            "sub01" => new Color(0.50f, 0.85f, 0.40f),
            "sub02" => new Color(0.40f, 0.60f, 0.95f),
            "sub03" => new Color(0.95f, 0.65f, 0.30f),
            "sub04" => new Color(0.90f, 0.35f, 0.35f),
            "sub05" => new Color(0.95f, 0.50f, 0.70f),
            "sub06" => new Color(0.95f, 0.90f, 0.40f),
            "sub07" => new Color(0.70f, 0.50f, 0.30f),
            "sub08" => new Color(0.65f, 0.45f, 0.85f),
            "sub09" => new Color(0.70f, 0.70f, 0.75f),
            "sub10" => new Color(0.40f, 0.85f, 0.90f),
            "sub11" => new Color(0.90f, 0.95f, 1.00f),
            "sub12" => new Color(0.30f, 0.60f, 0.50f),
            _ => new Color(0.80f, 0.80f, 0.80f),
        };

        public const int MaxDicas = 3;
        public const float CustoPorDica = 2f;

        public static readonly string[] NomesPacientes = { "Dr. Tomás", "Dra. Lia", "Dr. Breno" };

        public static string TipoAmigavel(string tipo) => tipo switch
        {
            "bacteria" => "bactéria",
            "virus" => "vírus",
            _ => "protozoário",
        };

        private static string TipoComArtigo(string tipo) => tipo switch
        {
            "bacteria" => "uma bactéria",
            "virus" => "um vírus",
            _ => "um protozoário",
        };

        // Dica 1: o tipo. Dica 2: o sintoma principal. Dica 3: quais ingredientes procurar.
        public static string TextoDica(Microrganismo m, Substancia especifica, Substancia sintoma, int numero) => numero switch
        {
            1 => $"Ele foi contaminado por {TipoComArtigo(m.Tipo)}. O remédio precisa combater esse tipo.",
            2 => $"O que mais incomoda é: {m.SintomaPaciente.SintomaExtra.ToLower()}. O remédio precisa aliviar isso.",
            _ => $"Procure por: {especifica?.Nome} e {sintoma?.Nome}.",
        };

        public static bool EhRemedio(string id) => id != null && id.StartsWith(PrefixoRemedio);

        public static string CriarRemedio(string especifica, string sintoma) =>
            $"{PrefixoRemedio}{especifica}+{sintoma}";

        public static (string Especifica, string Sintoma) PartesDoRemedio(string id)
        {
            var partes = id.Substring(PrefixoRemedio.Length).Split('+');
            return (partes[0], partes.Length > 1 ? partes[1] : "");
        }

        // As 3 primeiras substâncias combatem o microrganismo; as outras aliviam sintomas.
        public static bool EhEspecifica(string substanciaId) => substanciaId is "sub01" or "sub02" or "sub03";

        public static string NomeDoItem(PuzzleManager pm, string id)
        {
            if (!EhRemedio(id)) return pm?.ObterSubstancia(id)?.Nome ?? "";

            var (esp, sint) = PartesDoRemedio(id);
            return $"Remédio ({pm?.ObterSubstancia(esp)?.Nome} + {pm?.ObterSubstancia(sint)?.Nome})";
        }

        public static string InfoDoItem(PuzzleManager pm, string id)
        {
            if (EhRemedio(id)) return $"{NomeDoItem(pm, id)}. É só levar ao paciente certo!";

            var s = pm?.ObterSubstancia(id);
            return s == null ? "" : $"{s.Nome}: {s.Efeito.ToLower()}.";
        }

        public static Color CorDoItem(string id)
        {
            if (!EhRemedio(id)) return CorDaSubstancia(id);

            var (esp, sint) = PartesDoRemedio(id);
            return CorDaSubstancia(esp).Lerp(CorDaSubstancia(sint), 0.5f);
        }

        public static ResultadoRemedio AvaliarRemedio(Microrganismo m, string remedioId)
        {
            var (esp, sint) = PartesDoRemedio(remedioId);
            bool espOk = esp == m.CuraIngredientes[0];
            bool sintOk = sint == m.CuraIngredientes[1];

            if (espOk && sintOk) return ResultadoRemedio.Correto;
            if (espOk) return ResultadoRemedio.SoEspecificaCerta;
            if (sintOk) return ResultadoRemedio.SoSintomaCerto;
            return ResultadoRemedio.Errado;
        }

        public static string MensagemResultado(ResultadoRemedio r, string nome) => r switch
        {
            ResultadoRemedio.Correto => $"Funcionou! {nome} está bem melhor e já vai voltar ao trabalho.",
            ResultadoRemedio.SoEspecificaCerta => "Quase! Combate o microrganismo certo, mas não alivia o sintoma. Ajuste a outra parte.",
            ResultadoRemedio.SoSintomaCerto => "Quase! Alivia o sintoma, mas não combate o microrganismo. Ajuste a outra parte.",
            _ => "Hmm... acho que piorou! Esse remédio não combate o microrganismo nem alivia o sintoma.",
        };

        public static string MensagemMisturaInvalida(bool doisEspecificos) => doisEspecificos
            ? "Hmm... esse não parece seguro. Os dois só combatem microrganismos, e falta algo para aliviar o sintoma."
            : "Hmm... esse não parece seguro. Os dois só aliviam sintomas, e falta algo que combata o microrganismo.";

        // Cura de primeira vale 11. Cada tentativa errada antes tira 3 (mínimo 2). Cada dica tira 2.
        public static float PontosDaCura(int falhas, int dicas)
        {
            float pontos = Math.Max(MinimoCura, PontosCura - falhas * PenalidadePorFalha);
            return Math.Max(0f, pontos - dicas * CustoPorDica);
        }
    }
}