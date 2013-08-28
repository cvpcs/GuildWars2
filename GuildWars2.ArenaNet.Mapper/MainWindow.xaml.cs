using System;
using System.Windows;
using System.Windows.Controls;

namespace GuildWars2.ArenaNet.Mapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainControl.StartThreads();
        }

        protected override void OnClosed(EventArgs e)
        {
            MainControl.StopThreads();

            base.OnClosed(e);
        }
    }
}
