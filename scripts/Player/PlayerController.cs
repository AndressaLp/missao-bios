using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;
using MissaoBios.scripts.Interaction;

namespace MissaoBios.scripts.Player
{
    public partial class PlayerController : CharacterBody2D
    {
        [Export] public float Speed = 60f; // pixels/segundo
        [Export] public SpriteFrames FramesMenino;
        [Export] public SpriteFrames FramesMenina;

        private AnimatedSprite2D _sprite;
        private Label _promptInteragir;
        private Interactable _interactableProximo;
        private DialogueManager _dialogueManager;
        private Vector2 _ultimaDirecao = Vector2.Down;

        public override void _Ready()
        {
            _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

            _promptInteragir = GetNode<Label>("PromptInteragir");
            _dialogueManager = GetNodeOrNull<DialogueManager>("/root/DialogueManager");

            var raio = GetNode<Area2D>("RaioInteracao");
            raio.AreaEntered += OnAreaEntrouNoRaio;
            raio.AreaExited += OnAreaSaiuDoRaio;

            var saveManager = GetNode<SaveManager>("/root/SaveManager");
            bool ehMenina = saveManager.PerfilAtivo != null && saveManager.PerfilAtivo.Personagem == "menina";
            _sprite.SpriteFrames = ehMenina ? FramesMenina : FramesMenino;

            var gameManager = GetNodeOrNull<GameManager>("/root/GameManager");
            if (gameManager != null && !string.IsNullOrEmpty(gameManager.ProximoPontoEntrada))
            {
                var marker = GetParent().GetNodeOrNull<Marker2D>(gameManager.ProximoPontoEntrada);
                if (marker != null) Position = marker.Position;
                else GD.PrintErr($"[Player] Ponto de entrada não encontrado: '{gameManager.ProximoPontoEntrada}'");
                gameManager.ProximoPontoEntrada = "";
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            bool emDialogo = _dialogueManager != null && _dialogueManager.EmDialogo;

            Vector2 direcao = emDialogo ? Vector2.Zero : Input.GetVector("move_left", "move_right", "move_up", "move_down");

            Velocity = direcao * Speed;
            MoveAndSlide();

            AtualizarAnimacao(direcao);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (!@event.IsActionPressed("interagir")) return;

            bool emDialogo = _dialogueManager != null && _dialogueManager.EmDialogo;
            if (emDialogo) return; // deixa o DialogueUI cuidar de avançar a fala

            if (_interactableProximo != null)
            {
                _interactableProximo.Interagir();
                GetViewport().SetInputAsHandled(); // evita que esse mesmo clique também avance o diálogo
            }
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

        private void OnAreaEntrouNoRaio(Area2D area)
        {
            if (area is Interactable interactable)
            {
                _interactableProximo = interactable;
                _promptInteragir.Visible = true;
            }
        }

        private void OnAreaSaiuDoRaio(Area2D area)
        {
            if (area == _interactableProximo)
            {
                _interactableProximo = null;
                _promptInteragir.Visible = false;
            }
        }
    }
}