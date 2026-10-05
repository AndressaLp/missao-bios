using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace MissaoBios.scripts.Managers
{
    public partial class DialogueManager : Node
    {
        [Signal] public delegate void FalaAtualizadaEventHandler(string texto);
        [Signal] public delegate void DialogoEncerradoEventHandler();

        private readonly Dictionary<string, List<string>> _dialogos = new()
        {
            ["kael_hangar"] = new List<string>
            {
                "Você deve ser o aprendiz que chegou agora! Que bom te ver por aqui.",
                "As coisas andam complicadas na Estação Bios...",
                "Encontramos micro-organismos desconhecidos nas amostras do planeta Vitta. Eles contaminaram parte da estação, e alguns colegas ficaram doentes.",
                "Preciso da sua ajuda pra investigar tudo e colocar a estação de volta no ar.",
                "Vá até a sala logo ali na frente — é o Painel Central da estação. Te encontro lá.",
            },
            ["kael_painel"] = new List<string>
            {
                "Chegou rapidinho! Aqui é o Painel Central da estação.",
                "Uma coisa importante antes de começarmos: aqui, errar não é problema.",
                "Se você tentar algo e não der certo, eu vou te explicar o porquê — e você pode tentar de novo, quantas vezes precisar.",
                "Vê aquela barra ali no painel? Ela mostra como a estação está se recuperando. Cada desafio resolvido faz ela subir um pouco.",
                "Quando a missão terminar, esse painel também mostra um relatório com tudo que você descobriu.",
                "Vamos começar pelo Laboratório. Observe bem cada amostra antes de decidir o que fazer com ela.",
                "Se ficar com dúvida sobre como tudo funciona, pode voltar aqui e falar comigo de novo, tá bem?",
            },
            ["kael_lab_teaser"] = new List<string>
            {
                "Chegamos! Esse é o Laboratório.",
                "Na próxima vez, vamos aprender a investigar essas amostras estranhas.",
            },
        };

        private Queue<string> _filaAtual = new();
        public bool EmDialogo { get; private set; }

        public void IniciarDialogo(string dialogoId)
        {
            if (!_dialogos.TryGetValue(dialogoId, out var blocos))
            {
                GD.PushWarning($"DialogueManager: '{dialogoId}' não tem diálogo cadastrado.");
                return;
            }

            _filaAtual = new Queue<string>(blocos);
            EmDialogo = true;
            AvancarFala();
        }

        public void AvancarFala()
        {
            if (_filaAtual.Count == 0)
            {
                EmDialogo = false;
                EmitSignal(SignalName.DialogoEncerrado);
                return;
            }

            EmitSignal(SignalName.FalaAtualizada, _filaAtual.Dequeue());
        }
    }
}