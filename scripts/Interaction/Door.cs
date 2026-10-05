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

        private Sprite2D _sprite;

        public override void _Ready()
        {
            _sprite = GetNode<Sprite2D>("Sprite2D");
        }

        public override async void Interagir()
        {
            if (TexturaAberta != null)
                _sprite.Texture = TexturaAberta;

            if (!string.IsNullOrEmpty(CenaDestino))
            {
                if (!string.IsNullOrEmpty(PontoDeEntradaId))
                {
                    var gameManager = GetNode<GameManager>("/root/GameManager");
                    gameManager.ProximoPontoEntrada = PontoDeEntradaId;
                }

                await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
                GetTree().ChangeSceneToFile(CenaDestino);
            }
        }
    }
}