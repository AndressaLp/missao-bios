using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.Interaction
{
    public partial class Door : Interactable
    {
        [Export] public Texture2D TexturaAberta;
        [Export(PropertyHint.File, "*.tscn")] public string CenaDestino = "";
        [Export] public string PontoDeEntradaId = "";
        [Export] public bool ExigePuzzle1Concluido = false;
        [Export] public string DialogoTrancada = "";

        private Sprite2D _sprite;
        private bool _abrindo;

        public override void _Ready()
        {
            _sprite = GetNode<Sprite2D>("Sprite2D");
        }

        public override async void Interagir()
        {
            if (_abrindo) return;
            
            var gameManager = GetNode<GameManager>("/root/GameManager");

            if (ExigePuzzle1Concluido && !gameManager.Puzzle1Concluido)
            {
                if (!string.IsNullOrEmpty(DialogoTrancada))
                    GetNode<DialogueManager>("/root/DialogueManager").IniciarDialogo(DialogoTrancada);
                return;
            }

            if (TexturaAberta != null)
                _sprite.Texture = TexturaAberta;

            if (!string.IsNullOrEmpty(CenaDestino))
            {
                _abrindo = true;
                
                if (!string.IsNullOrEmpty(PontoDeEntradaId))
                {
                    gameManager.ProximoPontoEntrada = PontoDeEntradaId;
                }

                await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
                GetTree().ChangeSceneToFile(CenaDestino);
            }
        }
    }
}