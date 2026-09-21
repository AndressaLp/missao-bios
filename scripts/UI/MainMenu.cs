using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.UI
{
    public partial class MainMenu : Control
    {
        // Caminho temporário: ainda não existe a cena do Hangar/Painel Central.
        // Quando existir, trocar esse caminho.
        private const string CenaDeJogo = "res://scenes/station/sala_teste.tscn";

        private Button _botaoContinuarJogo;
        private Button _botaoSom;

        private Panel _painelNovoJogo;
        private LineEdit _campoNome;
        private Button _botaoMenino;
        private Button _botaoMenina;

        private Panel _painelEscolherJogador;
        private HBoxContainer _listaPerfis;

        private Panel _painelComoJogar;

        private SaveManager _saveManager;
        private AudioManager _audioManager;
        [Export] public PackedScene CenaCard;
        [Export] public Texture2D IconeMenino;
        [Export] public Texture2D IconeMenina;

        public override void _Ready()
        {
            _saveManager = GetNode<SaveManager>("/root/SaveManager");
            _audioManager = GetNode<AudioManager>("/root/AudioManager");

            GetNode<Button>("BotoesPrincipais/BotaoNovoJogo").Pressed += AbrirNovoJogo;
            _botaoContinuarJogo = GetNode<Button>("BotoesPrincipais/BotaoContinuarJogo");
            _botaoContinuarJogo.Pressed += ContinuarJogo;
            GetNode<Button>("BotoesPrincipais/BotaoEscolherJogador").Pressed += AbrirEscolherJogador;

            _botaoSom = GetNode<Button>("BotoesSecundarios/BotaoSom");
            _botaoSom.Pressed += AlternarSom;
            GetNode<Button>("BotoesSecundarios/BotaoComoJogar").Pressed += () => _painelComoJogar.Visible = true;

            _painelNovoJogo = GetNode<Panel>("PainelNovoJogo");
            _campoNome = GetNode<LineEdit>("PainelNovoJogo/CampoNome");
            _botaoMenino = GetNode<Button>("PainelNovoJogo/EscolhaPersonagem/BotaoMenino");
            _botaoMenina = GetNode<Button>("PainelNovoJogo/EscolhaPersonagem/BotaoMenina");
            GetNode<Button>("PainelNovoJogo/BotaoIniciar").Pressed += IniciarNovoJogo;
            GetNode<Button>("PainelNovoJogo/BotaoVoltarNovoJogo").Pressed += () => _painelNovoJogo.Visible = false;

            _painelEscolherJogador = GetNode<Panel>("PainelEscolherJogador");
            _listaPerfis = GetNode<HBoxContainer>("PainelEscolherJogador/ListaPerfis");
            GetNode<Button>("PainelEscolherJogador/BotaoVoltarEscolher").Pressed += () => _painelEscolherJogador.Visible = false;

            _painelComoJogar = GetNode<Panel>("PainelComoJogar");
            GetNode<Button>("PainelComoJogar/BotaoFecharComoJogar").Pressed += () => _painelComoJogar.Visible = false;

            AtualizarBotaoContinuar();
        }

        private void AtualizarBotaoContinuar()
        {
            _botaoContinuarJogo.Disabled = _saveManager.ObterUltimoPerfil() == null;
        }

        private void AbrirNovoJogo()
        {
            _campoNome.Text = "";
            _painelNovoJogo.Visible = true;
        }

        private void IniciarNovoJogo()
        {
            string nome = _campoNome.Text.Trim();
            if (nome.Length == 0) nome = "Aprendiz";

            string personagem = _botaoMenina.ButtonPressed ? "menina" : "menino";

            var perfil = _saveManager.CriarPerfil(nome, personagem);
            _saveManager.DefinirPerfilAtivo(perfil);

            GetTree().ChangeSceneToFile(CenaDeJogo);
        }

        private void ContinuarJogo()
        {
            var ultimo = _saveManager.ObterUltimoPerfil();
            if (ultimo == null) return;

            _saveManager.DefinirPerfilAtivo(ultimo);
            GetTree().ChangeSceneToFile(CenaDeJogo);
        }

        private void AbrirEscolherJogador()
        {
            foreach (Node filho in _listaPerfis.GetChildren())
                filho.QueueFree();

            foreach (var perfil in _saveManager.ListarPerfisRecentes(2))
            {
                var card = CenaCard.Instantiate<PerfilCard>();
                card.Configurar(perfil, IconeMenino, IconeMenina);
                card.Pressed += () => SelecionarPerfil(card.Perfil);
                _listaPerfis.AddChild(card);
            }

            _painelEscolherJogador.Visible = true;
        }

        private void SelecionarPerfil(PerfilJogador perfil)
        {
            _saveManager.DefinirPerfilAtivo(perfil);
            GetTree().ChangeSceneToFile(CenaDeJogo);
        }

        private void AlternarSom()
        {
            _audioManager.AlternarSom();
            _botaoSom.Text = _audioManager.SomAtivado ? "🔊" : "🔇";
        }
    }
}