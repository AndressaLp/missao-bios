using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.Puzzle
{
    // Representa uma amostra individual dentro do Laboratório.
    // Guarda qual microrganismo ela tem e se já foi resolvida.
    public class Amostra
    {
        public string Id;
        public string NomeMicrorganismo;
        public MetodoLaboratorio MetodoCorreto;
        public bool Resolvida = false;
        public bool foiTestada;
    }

    public enum MetodoLaboratorio
    {
        Calor,
        Frio,
        Luz,
        Misturar
    }
}