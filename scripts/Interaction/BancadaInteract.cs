using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;
using MissaoBios.scripts.UI;

namespace MissaoBios.scripts.Interaction
{
    public partial class BancadaInteract : Interactable
    {
        public override void Interagir()
        {
            var gm = GetNode<GameManager>("/root/GameManager");
            var pm = GetNode<PuzzleManager>("/root/PuzzleManager");
            var dm = GetNode<DialogueManager>("/root/DialogueManager");

            bool apresentou = gm.MissaoConcluida("falar_jack") || pm.PacientesAtuais.Any(p => p.Examinado);
            GD.Print($"[Bancada] chamada | apresentou={apresentou} | rodadaFinalizada={pm.RodadaEnfermariaFinalizada}"); // temporário

            if (!apresentou)
            {
                dm.IniciarFalas("Uma bancada de laboratório cheia de equipamentos.", "Melhor falar com o Dr. Jack antes de usar.");
                return;
            }

            if (pm.RodadaEnfermariaFinalizada)
            {
                dm.IniciarFalas("Já terminamos as curas por aqui.");
                return;
            }

            var ui = GetNodeOrNull<BancadaUI>("/root/BancadaUIAutoload");
            if (ui == null)
            {
                GD.PrintErr("[Bancada] Autoload 'BancadaUIAutoload' não encontrado (Project Settings → Autoload).");
                dm.IniciarFalas("A bancada não está funcionando agora.");
                return;
            }

            ui.Abrir();

            // if (!gm.MissaoConcluida("falar_jack"))
            // {
            //     dm.IniciarFalas("Uma bancada de laboratório cheia de equipamentos.", "Melhor falar com o Dr. Jack antes de usar.");
            //     return;
            // }

            // if (pm.RodadaEnfermariaFinalizada)
            // {
            //     dm.IniciarFalas("Já terminamos as curas por aqui.");
            //     return;
            // }

            // GetNodeOrNull<BancadaUI>("/root/BancadaUIAutoload")?.Abrir();
        }
    }
}