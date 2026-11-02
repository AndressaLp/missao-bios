using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissaoBios.scripts.Managers
{
    public class SorteioService<T>
    {
        private readonly List<T> _itensOriginais;
        private readonly Queue<T> _fila = new();
        private readonly Random _random = new();

        public SorteioService(List<T> itens)
        {
            _itensOriginais = new List<T>(itens);
            Reembaralhar();
        }

        public T Sortear()
        {
            if (_fila.Count == 0) Reembaralhar();
            return _fila.Dequeue();
        }

        private void Reembaralhar()
        {
            var copia = new List<T>(_itensOriginais);
            for (int i = copia.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (copia[i], copia[j]) = (copia[j], copia[i]);
            }
            _fila.Clear();
            foreach (var item in copia) _fila.Enqueue(item);
        }
    }
}