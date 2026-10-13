using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MissaoBios.scripts.Managers;

namespace MissaoBios.scripts.UI
{
    public partial class PerfilCard : Button
    {
        public PerfilJogador Perfil { get; private set; }

        public void Configurar(PerfilJogador perfil, Texture2D iconeMenino, Texture2D iconeMenina)
        {
            Perfil = perfil;

            var labelNome = GetNode<Label>("LabelNome");
            var iconPersonagem = GetNode<TextureRect>("IconPersonagem");

            labelNome.Text = perfil.Nome;
            iconPersonagem.Texture = perfil.Personagem == "menina" ? iconeMenina : iconeMenino;
        }
    }
}