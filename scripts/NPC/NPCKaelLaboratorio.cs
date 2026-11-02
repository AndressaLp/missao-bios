using System;
using Godot;
using MissaoBios.scripts.Interaction;
using MissaoBios.scripts.Managers;

public partial class NPCKaelLaboratorio : Interactable
{
    public override void Interagir()
    {
        var pm = GetNode<PuzzleManager>("/root/PuzzleManager");
        var dm = GetNode<DialogueManager>("/root/DialogueManager");
        var gm = GetNode<GameManager>("/root/GameManager");

        bool todas = pm.TodasAsAmostrasResolvidas();

        if (todas && !pm.RodadaFinalizada)
        {
            // Terminou uma rodada (a primeira ou uma repetição): a Dra. Kael encerra.
            pm.RodadaFinalizada = true;
            bool primeiraVez = !gm.Puzzle1Concluido;
            if (primeiraVez)
            {
                gm.Puzzle1Concluido = true;
                gm.AvancarProgresso(34f); // só na primeira vez
            }

            AoFinalizar(dm, () => gm.ConcluirMissao("concluir_lab"));
            dm.IniciarDialogo(primeiraVez ? "kael_lab_conclusao" : "kael_lab_replay_conclusao");
        }
        else if (todas)
        {
            // Rodada já encerrada: oferece jogar de novo.
            dm.IniciarDialogoComEscolha("kael_lab_pergunta_repetir", "Sim", "Não", querRepetir =>
            {
                if (querRepetir) ComecarNovaRodada(pm, dm, gm);
                else dm.IniciarDialogo("kael_lab_replay_nao");
            });
        }
        else if (pm.AlgumaAmostraResolvida())
        {
            dm.IniciarDialogo("kael_lab_lembrete");
        }
        else if (gm.Puzzle1Concluido)
        {
            dm.IniciarDialogo("kael_lab_replay_lembrete");
        }
        else
        {
            AoFinalizar(dm, () => gm.ConcluirMissao("falar_kael_lab"));
            dm.IniciarDialogo("kael_lab_intro");
        }
    }

    private static void ComecarNovaRodada(PuzzleManager pm, DialogueManager dm, GameManager gm)
    {
        gm.ZerarPontosDoPuzzle("puzzle1");   // a nova tentativa substitui a anterior
        pm.ReiniciarRodadaLaboratorio();     // novas amostras serão sorteadas ao abrir o microscópio
        gm.ReabrirMissoes("analisar_amostras", "concluir_lab", "Analise as amostras no microscópio (0/3)");
        dm.IniciarDialogo("kael_lab_replay_inicio");
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
}