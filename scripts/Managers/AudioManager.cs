using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using System.Text.Json;

namespace MissaoBios.scripts.Managers
{

    public partial class AudioManager : Node
    {
        public bool SomAtivado { get; private set; } = true;

        public override void _Ready()
        {
            // TODO: configurar players de música e SFX (próxima etapa)
        }

        public void AlternarSom()
        {
            SomAtivado = !SomAtivado;
            int busIndex = AudioServer.GetBusIndex("Master");
            AudioServer.SetBusMute(busIndex, !SomAtivado);
        }

        public void TocarMusica(string faixaId)
        {
            // TODO: implementar quando os arquivos de áudio estiverem prontos
        }

        public void TocarSom(string somId)
        {
            // TODO: implementar quando os arquivos de áudio estiverem prontos
        }
    }
}