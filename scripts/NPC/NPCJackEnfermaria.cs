using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissaoBios.scripts.Interaction;
using MissaoBios.scripts.Managers;
using Godot;

namespace MissaoBios.scripts.NPC
{
    public partial class NPCJackEnfermaria : Interactable
    {
        public override void Interagir()
        {
            var pm = GetNode<PuzzleManager>("/root/PuzzleManager");
            var dm = GetNode<DialogueManager>("/root/DialogueManager");
            var gm = GetNode<GameManager>("/root/GameManager");

            if (!gm.MissaoConcluida("falar_jack"))
            {
                AoFinalizar(dm, () => gm.ConcluirMissao("falar_jack"));
                dm.IniciarDialogo("jack_intro");
                return;
            }

            var pacientes = pm.PacientesAtuais;
            bool todosCurados = pacientes.Count == 3 && pacientes.All(p => p.Curado);
            bool todosExaminados = pacientes.Count == 3 && pacientes.All(p => p.Examinado);

            if (todosCurados && !pm.RodadaEnfermariaFinalizada)
            {
                // Terminou uma rodada (a primeira ou uma repetição): o Dr. Jack encerra.
                pm.RodadaEnfermariaFinalizada = true;
                bool primeiraVez = !gm.Puzzle2Concluido;
                if (primeiraVez)
                {
                    gm.Puzzle2Concluido = true;
                    gm.AvancarProgresso(33f); // só na primeira vez
                }

                AoFinalizar(dm, () =>
                {
                    gm.ConcluirMissao("concluir_enfermaria");
                    if(primeiraVez) MostrarAvisoFimDaDemo(dm);
                });
                dm.IniciarDialogo(primeiraVez ? "jack_conclusao" : "jack_replay_conclusao");
            }
            else if (todosCurados)
            {
                dm.IniciarDialogoComEscolha("jack_pergunta_repetir", "Sim", "Não", querRepetir =>
                {
                    if (querRepetir) ComecarNovaRodada(pm, dm, gm);
                    else dm.IniciarDialogo("jack_replay_nao");
                });
            }
            else if (gm.Puzzle2Concluido && !pacientes.Any(p => p.Examinado))
            {
                dm.IniciarDialogo("jack_replay_lembrete");
            }
            else if (todosExaminados && !gm.MissaoConcluida("falar_jack_ingredientes"))
            {
                AoFinalizar(dm, () => gm.ConcluirMissao("falar_jack_ingredientes"));
                dm.IniciarDialogo("jack_ingredientes");
            }
            else if (todosExaminados)
            {
                dm.IniciarDialogo("jack_lembrete_cura");
            }
            else
            {
                dm.IniciarDialogo("jack_lembrete");
            }
        }

        private static void ComecarNovaRodada(PuzzleManager pm, DialogueManager dm, GameManager gm)
        {
            gm.ZerarPontosDoPuzzle("puzzle2");   // a nova tentativa substitui a anterior
            pm.ReiniciarRodadaEnfermaria();      // novos pacientes e frascos reembaralhados
            gm.LimparInventario();
            gm.ReabrirMissoes("examinar_pacientes", "concluir_enfermaria", "Examine os pacientes (0/3)");
            dm.IniciarDialogo("jack_replay_inicio");
        }

        private static void AoFinalizar(DialogueManager dm, Action acao)
        {
            void Handler()
            {
                dm.DialogoEncerrado -= Handler;
                acao();
            }
            dm.DialogoEncerrado += Handler;
        }

        // Mensagem de "o jogo ainda não terminou", com duas opções.
        private void MostrarAvisoFimDaDemo(DialogueManager dm)
        {
            dm.IniciarFalasComEscolha(
                new[]
                {
                    "Parabéns! Você concluiu o Laboratório e a Enfermaria.",
                    "O jogo ainda não terminou: a próxima etapa, a Purificação da Água, está em desenvolvimento.",
                    "O que você quer fazer agora?",
                },
                "Sair do jogo", "Continuar",
                sair => { if (sair) SairDoJogo(); });
        }

        private void SairDoJogo()
        {
            GetNodeOrNull<SaveManager>("/root/SaveManager")?.SalvarProgresso();
            GetTree().ChangeSceneToFile("res://scenes/main/MainMenu.tscn");
        }
    }
}