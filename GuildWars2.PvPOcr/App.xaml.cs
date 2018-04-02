using System;
using System.IO;
using System.Windows;

namespace GuildWars2.PvPOcr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => this.LogUnhandledException(e.ExceptionObject as Exception);
            this.DispatcherUnhandledException += (s, e) => this.LogUnhandledException(e.Exception);

            base.OnStartup(args);
        }

        private void LogUnhandledException(Exception e)
        {
            if (e != null)
            {
                File.WriteAllText("./err.txt", e.ToString());
            }
        }
    }
}
