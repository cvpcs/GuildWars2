using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using GuildWars2.PvPCasterToolbox.Configuration;
using GuildWars2.PvPCasterToolbox.GameState;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace GuildWars2.PvPCasterToolbox.TabPages
{
    public partial class SoundEffects : TabItem
    {
        private static readonly IDictionary<string, string> SoundEffectsFileFilters = new Dictionary<string, string>
        {
            ["Audio files"] = "*.mp3;*.aac;*.wav",
            ["MP3 files"] = "*.mp3",
            ["AAC files"] = "*.aac",
            ["WAV files"] = "*.wav"
        };

        public AppConfig AppConfig { get; private set; }

        private ILogger logger;

        public SoundEffects(GameStateManager gameStateManager,
                            AppConfig appConfig,
                            ILogger<SoundEffects> logger)
        {
            AppConfig = appConfig;

            this.logger = logger;

            InitializeComponent();

            gameStateManager.ScoresRead += this.OnScoresRead;
        }

        // TODO: ICommand?
        public void SelectRedKillSoundEffect_Clicked(object sender, EventArgs args)
        {
            if (TrySelectSoundEffect(out string path))
            {
                AppConfig.RedKillSoundEffect = path;
                this.logger.LogInformation($"Selected red kill sound effect {AppConfig.RedKillSoundEffect}");
            }
        }

        public void SelectBlueKillSoundEffect_Clicked(object sender, EventArgs args)
        {
            if (TrySelectSoundEffect(out string path))
            {
                AppConfig.BlueKillSoundEffect = path;
                this.logger.LogInformation($"Selected blue kill sound effect {AppConfig.BlueKillSoundEffect}");
            }
        }

        private bool TrySelectSoundEffect(out string path)
        {
            var dialog = new OpenFileDialog
            {
                Filter = string.Join("|", SoundEffectsFileFilters.Select(kvp => $"{kvp.Key} ({kvp.Value})|{kvp.Value}")),
                Multiselect = false,
                Title = "Select sound effect"
            };

            bool? result = dialog.ShowDialog();
            path = dialog.FileName;
            return result == true;
        }

        private void OnScoresRead(IGameState state)
        {
            if (state.Red.KillsDelta > 0)
            {
                PlaySoundEffect(AppConfig.RedKillSoundEffect);
            }

            if (state.Blue.KillsDelta > 0)
            {
                PlaySoundEffect(AppConfig.BlueKillSoundEffect);
            }
        }

        private static void PlaySoundEffect(string path)
        {
            if (File.Exists(path))
            {
                var mediaPlayer = new MediaPlayer();
                mediaPlayer.Open(new Uri(Path.GetFullPath(path)));
                mediaPlayer.Play();
            }
        }
    }
}
