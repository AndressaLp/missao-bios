using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace MissaoBios.scripts.Player
{
    public partial class PlayerController : CharacterBody2D
    {
         [Export] public float Speed = 60f; // pixels/segundo

        private AnimatedSprite2D _sprite;
        private Vector2 _ultimaDirecao = Vector2.Down;

        public override void _Ready()
        {
            _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        }

        public override void _PhysicsProcess(double delta)
        {
            Vector2 direcao = Input.GetVector("move_left", "move_right", "move_up", "move_down");

            Velocity = direcao * Speed;
            MoveAndSlide();

            AtualizarAnimacao(direcao);
        }

        private void AtualizarAnimacao(Vector2 direcao)
        {
            bool movendo = direcao.Length() > 0.1f;

            if (movendo)
            {
                _ultimaDirecao = direcao;
            }

            // Decide se a direção predominante é vertical (cima/baixo) ou lateral (esquerda/direita)
            if (Mathf.Abs(_ultimaDirecao.Y) >= Mathf.Abs(_ultimaDirecao.X))
            {
                if (_ultimaDirecao.Y > 0)
                    _sprite.Play(movendo ? "walk_down" : "idle_down");
                else
                    _sprite.Play(movendo ? "walk_up" : "idle_up");
            }
            else
            {
                if (_ultimaDirecao.X > 0)
                    _sprite.Play(movendo ? "walk_right" : "idle_right");
                else
                    _sprite.Play(movendo ? "walk_left" : "idle_left");
            }
        }
    }
}