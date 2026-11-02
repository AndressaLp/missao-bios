using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.Interaction
{
    public partial class PontoIngredienteInteract : Interactable
    {
        [Export] public string PontoId = ""; // ex.: "hangar_1", "lab_2", "enf_3"

        private GameManager _gm;
        private PuzzleManager _pm;

        public override void _Ready()
        {
            _gm = GetNodeOrNull<GameManager>("/root/GameManager");
            _pm = GetNodeOrNull<PuzzleManager>("/root/PuzzleManager");
            if (_gm != null) _gm.MissoesAtualizadas += AtualizarAtivo;
            AtualizarAtivo();
        }

        private bool Ativo() =>
            _gm != null && _pm != null
            && _gm.MissaoConcluida("falar_jack_ingredientes")
            && !_pm.RodadaEnfermariaFinalizada;

        private void AtualizarAtivo()
        {
            if (!IsInstanceValid(this)) return;

            bool ativo = Ativo();
            Visible = ativo;
            GetNodeOrNull<CollisionShape2D>("CollisionShape2D")?.SetDeferred("disabled", !ativo);
        }

        public override void _ExitTree()
        {
            // O GameManager vive o jogo todo; este frasco some junto com a sala.
            if (_gm != null) _gm.MissoesAtualizadas -= AtualizarAtivo;
        }

        public override void Interagir()
        {
            if (!Ativo()) return;

            var dm = GetNode<DialogueManager>("/root/DialogueManager");

            var substancia = _pm.ObterSubstanciaDoPonto(PontoId);
            if (substancia == null)
            {
                GD.PrintErr($"[Frasco] Ponto '{PontoId}' sem ingrediente. Confira o PontoId e a lista no PuzzleManager.");
                dm.IniciarFalas("Esse frasco está vazio.");
                return;
            }

            string info = $"{substancia.Nome}: {substancia.Efeito.ToLower()}.";

            if (_gm.TemItem(substancia.Id))
            {
                dm.IniciarFalas(info, "Você já está carregando um desses.");
                return;
            }

            if (_gm.InventarioCheio)
            {
                dm.IniciarFalas(info, "Seu inventário está cheio! Clique num item lá embaixo para descartá-lo.");
                return;
            }

            dm.IniciarFalasComEscolha(
                new[] { info, "Quer pegar esse ingrediente?" },
                "Pegar", "Deixar",
                pegar => { if (pegar) _gm.AdicionarItem(substancia.Id); });
        }
    }
}