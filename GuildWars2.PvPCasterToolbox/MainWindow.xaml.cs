using System;
using System.Windows;
using GuildWars2.PvPCasterToolbox.TabPages;
using Microsoft.Extensions.DependencyInjection;

namespace GuildWars2.PvPCasterToolbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            Pages.Items.Add(ActivatorUtilities.GetServiceOrCreateInstance<GameTracking>(serviceProvider));
            Pages.Items.Add(ActivatorUtilities.GetServiceOrCreateInstance<ConsoleLog>(serviceProvider));
        }
    }
}
