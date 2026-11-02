using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.Puzzle.Laboratorio
{
    public class SintomaPaciente
    {
        public string ManchaVisual { get; set; } = "nenhuma";
        public float TemperaturaC { get; set; }
        public int FrequenciaCardiacaBpm { get; set; }
        public string SintomaExtra { get; set; } = "";
    }

    public class Microrganismo
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Formato { get; set; }
        public string Cor { get; set; }
        public string Comportamento { get; set; }
        public string PistaMetodo { get; set; }
        public string Ambiente { get; set; }
        public string MetodoCorreto { get; set; }
        public List<string> CuraIngredientes { get; set; }
        public SintomaPaciente SintomaPaciente { get; set; } = new();
    }

    public class Substancia
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Efeito { get; set; }
        public string Utilidade { get; set; }
    }

    public class DadosJogo
    {
        public List<Microrganismo> Microrganismos { get; set; }
        public List<Substancia> SubstanciasCura { get; set; }
    }

    public class AmostraEmJogo
    {
        public Microrganismo Dados;
        public bool Resolvida;
        public QualidadeResposta? Resposta;
        
        public int DicasUsadas;
    }

    public class PacienteEmJogo
    {
        public int Posicao;            // 1, 2 ou 3 (qual cama)
        public Microrganismo Dados;    // o microrganismo que contaminou o paciente
        public bool Examinado;
        public bool Curado;
        public int DicasUsadas;
        public int Tentativas;         // usado nas próximas rodadas
    }
}