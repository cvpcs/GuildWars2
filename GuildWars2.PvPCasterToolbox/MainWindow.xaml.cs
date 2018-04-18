using System;
using System.Windows;
using GuildWars2.PvPCasterToolbox.Configuration;
using GuildWars2.PvPCasterToolbox.TabPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace GuildWars2.PvPCasterToolbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DefaultExt = ".json";
        private const string Filter = "JSON Files (*.json)|*.json";

        private AppConfig appConfig;
        private ILogger logger;

        public MainWindow(IServiceProvider serviceProvider, AppConfig appConfig, ILogger<MainWindow> logger)
        {
            this.appConfig = appConfig;
            this.logger = logger;

            InitializeComponent();

            Pages.Items.Add(ActivatorUtilities.GetServiceOrCreateInstance<GameTracking>(serviceProvider));
            Pages.Items.Add(ActivatorUtilities.GetServiceOrCreateInstance<ScoreBars>(serviceProvider));
            Pages.Items.Add(ActivatorUtilities.GetServiceOrCreateInstance<ConsoleLog>(serviceProvider));
        }

        // TODO: use ICommand model?
        public void LoadConfiguration_Clicked(object sender, EventArgs args)
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
                try
                {
                    this.appConfig.Load(dialog.FileName);
                    this.logger.LogInformation($"Loaded configuration from {dialog.FileName}");
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, string.Empty);
                }
            }
        }

        // TODO: use ICommand model?
        public void SaveConfiguration_Clicked(object sender, EventArgs args)
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
                try
                {
                    this.appConfig.Save(dialog.FileName);
                    this.logger.LogInformation($"Saved configuration to {dialog.FileName}");
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, string.Empty);
                }
            }
        }

        // TODO: use ICommand model?
        public void Exit_Clicked(object sender, EventArgs args)
            => this.Close();
    }
}
