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
                "Encontramos microrganismos desconhecidos nas amostras do planeta Vitta. Eles contaminaram parte da estação, e alguns colegas ficaram doentes.",
                "Preciso da sua ajuda pra investigar tudo e colocar a estação de volta no ar.",
                "Vá até a sala logo ali na frente, é o Painel Central da estação. Te encontro lá.",
            },
            ["kael_painel"] = new List<string>
            {
                "Chegou rapidinho! Aqui é o Painel Central da estação.",
                "Uma coisa importante antes de começarmos: aqui, errar não é problema.",
                "Se você tentar algo e não der certo, eu vou te explicar o porquê e você pode tentar de novo, quantas vezes precisar.",
                "Vê aquela barra ali no painel? Ela mostra como a estação está se recuperando. Cada desafio resolvido faz ela subir um pouco.",
                "Quando a missão terminar, esse painel também mostra um relatório com tudo que você descobriu.",
                "Vamos começar pelo Laboratório. Observe bem cada amostra antes de decidir o que fazer com ela.",
                "Se ficar com dúvida sobre como tudo funciona, pode voltar aqui e falar comigo de novo, tá bem?",
            },
            ["kael_lab_intro"] = new List<string>
            {
                "Chegamos! Esse é o Laboratório, onde investigamos os microrganismos de Vitta.",
                "Ali na bancada tem um microscópio com 3 amostras misteriosas.",
                "Escolha uma amostra pra observar. Você vai ver o tamanho, a cor e o formato dela.",
                "Depois, escolha o método que você acha que elimina esse microrganismo.",
                "Preste atenção nas pistas, nem todo método funciona em todo microrganismo!",
                "Pode errar sem medo, mas escolhas certas valem mais pontos. Boa sorte!",
            },
            ["kael_lab_lembrete"] = new List<string>
            {
                "Ainda faltam amostras pra analisar no microscópio. Vai lá!",
            },
            ["kael_lab_conclusao"] = new List<string>
            {
                "Você analisou as 3 amostras! Muito bem, pesquisador(a).",
                "Vou registrar tudo isso no Painel Central.",
                "Agora, vá para a enfermaria, estão precisando da sua ajuda!"
            },
            ["kael_lab_pergunta_repetir"] = new List<string>
            {
                "Você já concluiu o desafio do Laboratório!",
                "Quer analisar novas amostras e tentar de novo?",
            },
            ["kael_lab_replay_inicio"] = new List<string>
            {
                "Ótimo! Trouxe novas amostras para o microscópio.",
                "Dessa vez, tente descobrir o melhor método para cada uma.",
            },
            ["kael_lab_replay_nao"] = new List<string>
            {
                "Tudo bem! Se quiser tentar de novo, é só falar comigo.",
            },
            ["kael_lab_replay_lembrete"] = new List<string>
            {
                "As novas amostras já estão no microscópio. Boa sorte!",
            },
            ["kael_lab_replay_conclusao"] = new List<string>
            {
                "Você analisou as novas amostras! Atualizei sua pontuação no Painel Central.",
            },

            ["porta_enfermaria_trancada"] = new List<string>
            {
                "A Enfermaria ainda está fechada.",
                "Termine o desafio do Laboratório com a Dra. Kael primeiro!",
            },
            ["jack_intro"] = new List<string>
            {
                "Ah, você deve ser quem a Dra. Kael mencionou! Seja bem-vindo(a) à Enfermaria.",
                "Aqui cuidamos dos colegas que foram contaminados pelos microrganismos.",
                "Cada paciente tem sintomas diferentes: febre, coração acelerado, manchas na pele...",
                "Chegue perto de cada cama e examine com atenção. Os microrganismos do Laboratório podem te dar pistas!",
                "Depois, procure pela estação os ingredientes certos para preparar a cura.",
                "Pode errar sem medo: o remédio vai mudar de cor ou soltar fumaça, e a gente tenta de novo.",
                "Se ficar com dúvida, é só voltar e falar comigo.",
            },
            ["jack_lembrete"] = new List<string>
            {
                "Examine cada paciente com calma. Eu estou aqui se precisar!",
            },
            ["jack_ingredientes"] = new List<string>
            {
                "Muito bem! Agora você já sabe do que cada paciente precisa.",
                "Para preparar as curas, precisamos de ingredientes. Eles estão espalhados pela estação, dentro de frascos.",
                "Procure pelas salas. Chegue perto de um frasco para ver o que tem dentro e escolha se quer pegar.",
                "Você só consegue carregar 3 itens por vez. Se o inventário encher, clique num item para descartá-lo.",
                "Com os ingredientes certos, use a bancada aqui na enfermaria para misturar o remédio.",
                "Depois é só entregar o remédio ao paciente. Se errar, a gente ajusta e tenta de novo!",
            },

            ["jack_lembrete_cura"] = new List<string>
            {
                "Já examinamos todos! Agora prepare as curas na bancada e leve cada remédio ao paciente certo.",
                "Lembre-se: o remédio precisa de uma parte que combata o microrganismo e outra que alivie o sintoma.",
            },
            ["jack_conclusao"] = new List<string>
            {
                "Todos os pacientes foram curados! Excelente trabalho.",
                "Os cientistas já estão voltando aos seus postos, e os sistemas da estação estão voltando a funcionar.",
                "Confira o progresso no Painel Central.",
            },
            ["jack_pergunta_repetir"] = new List<string>
            {
                "Você já curou todos os pacientes!",
                "Quer atender novos pacientes e tentar de novo?",
            },
            ["jack_replay_inicio"] = new List<string>
            {
                "Ótimo! Chegaram novos pacientes na enfermaria.",
                "Examine cada um com atenção. Os sintomas são diferentes desta vez!",
            },
            ["jack_replay_nao"] = new List<string>
            {
                "Tudo bem! Se quiser tentar de novo, é só falar comigo.",
            },
            ["jack_replay_lembrete"] = new List<string>
            {
                "Os novos pacientes já estão nas camas. Examine cada um com atenção!",
            },
            ["jack_replay_conclusao"] = new List<string>
            {
                "Você curou os novos pacientes! Atualizei sua pontuação no Painel Central.",
            },
        };

        private Queue<string> _filaAtual = new();
        public bool EmDialogo { get; private set; }
        [Signal] public delegate void EscolhaSolicitadaEventHandler(string textoSim, string textoNao);

        private Action<bool> _aoEscolher;
        private string _textoSim = "Sim";
        private string _textoNao = "Não";
        public bool AguardandoEscolha { get; private set; }

        public void IniciarDialogo(string dialogoId)
        {
            _aoEscolher = null;
            Comecar(dialogoId);
        }

        // Toca o diálogo e, no fim, mostra dois botões. O resultado chega no aoEscolher (true = Sim).
        public void IniciarDialogoComEscolha(string dialogoId, string textoSim, string textoNao, Action<bool> aoEscolher)
        {
            _aoEscolher = aoEscolher;
            _textoSim = textoSim;
            _textoNao = textoNao;
            Comecar(dialogoId);
        }

        public void IniciarFalas(params string[] falas)
        {
            _aoEscolher = null;
            ComecarComFalas(new List<string>(falas));
        }

        public void IniciarFalasComEscolha(string[] falas, string textoSim, string textoNao, Action<bool> aoEscolher)
        {
            _aoEscolher = aoEscolher;
            _textoSim = textoSim;
            _textoNao = textoNao;
            ComecarComFalas(new List<string>(falas));
        }

        private void Comecar(string dialogoId)
        {
            if (!_dialogos.TryGetValue(dialogoId, out var blocos))
            {
                GD.PushWarning($"DialogueManager: '{dialogoId}' não tem diálogo cadastrado.");
                _aoEscolher = null;
                return;
            }
            ComecarComFalas(blocos);
        }

        private void ComecarComFalas(List<string> falas)
        {
            _filaAtual = new Queue<string>(falas);
            AguardandoEscolha = false;
            EmDialogo = true;
            AvancarFala();
        }

        public void AvancarFala()
        {
            if (AguardandoEscolha) return; // espera o clique num dos botões

            if (_filaAtual.Count == 0)
            {
                if (_aoEscolher != null)
                {
                    AguardandoEscolha = true;
                    EmitSignal(SignalName.EscolhaSolicitada, _textoSim, _textoNao);
                    return;
                }

                EmDialogo = false;
                EmitSignal(SignalName.DialogoEncerrado);
                return;
            }

            EmitSignal(SignalName.FalaAtualizada, _filaAtual.Dequeue());
        }

        public void Escolher(bool sim)
        {
            if (!AguardandoEscolha) return;

            AguardandoEscolha = false;
            var callback = _aoEscolher;
            _aoEscolher = null;
            EmDialogo = false;
            EmitSignal(SignalName.DialogoEncerrado);
            callback?.Invoke(sim);
        }
    }
}