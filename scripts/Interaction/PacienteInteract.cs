using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.UI;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.Interaction
{
    public partial class PacienteInteract : Interactable
    {
        [Export] public int Posicao = 1; // 1, 2 ou 3
        [Export] public Texture2D CamaComPaciente;  // a cama com este paciente
        [Export] public Texture2D CamaVazia;        // a cama depois da cura

        private Sprite2D _sprite;
        private bool _curadoAnterior;

        public override void _Ready()
        {
            _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
            if (_sprite == null) GD.PrintErr("[Paciente] Nó 'Sprite2D' não encontrado.");
            AtualizarVisual(true);
        }

        public override void _Process(double delta) => AtualizarVisual(false);

        private void AtualizarVisual(bool forcar)
        {
            var pm = GetNodeOrNull<PuzzleManager>("/root/PuzzleManager");
            bool curado = pm != null && Posicao >= 1 && Posicao <= pm.PacientesAtuais.Count
                        && pm.PacientesAtuais[Posicao - 1].Curado;

            if (!forcar && curado == _curadoAnterior) return;
            _curadoAnterior = curado;

            var textura = curado ? CamaVazia : CamaComPaciente;
            if (_sprite != null && textura != null) _sprite.Texture = textura;
        }

        public override void Interagir()
        {
            GetNodeOrNull<PacienteUI>("/root/PacienteUIAutoload")?.Abrir(Posicao);
        }
    }
}