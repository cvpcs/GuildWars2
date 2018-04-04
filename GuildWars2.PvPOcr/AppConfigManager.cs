using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace GuildWars2.PvPOcr
{
    public class AppConfigManager
    {
        private const string DefaultExt = ".json";
        private const string Filter = "JSON Files (*.json)|*.json";

        private ILogger logger;

        public AppConfigManager(ILogger logger)
            => this.logger = logger;

        public bool TryLoad(out AppConfig config)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = DefaultExt,
                Filter = Filter,
                Multiselect = false,
                Title = "Load configuration"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return TryLoad(dialog.FileName, out config);
            }

            config = null;
            return false;
        }

        public bool TryLoad(string filename, out AppConfig config)
        {
            try
            {
                config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(filename));
                this.logger.LogInformation($"Loaded configuration from {filename}");
                return true;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, string.Empty);
            }

            config = null;
            return false;
        }

        public bool TrySave(AppConfig config)
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = DefaultExt,
                Filter = Filter,
                Title = "Save configuration"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return TrySave(dialog.FileName, config);
            }

            return false;
        }

        public bool TrySave(string filename, AppConfig config)
        {
            try
            {
                File.WriteAllText(filename, JsonConvert.SerializeObject(config));
                this.logger.LogInformation($"Saved configuration to {filename}");
                return true;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, string.Empty);
            }

            return false;
        }
    }
}
