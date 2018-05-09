using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GuildWars2.PvPCasterToolbox.Configuration;
using GuildWars2.PvPCasterToolbox.GameState;
using GuildWars2.PvPCasterToolbox.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tesseract;

namespace GuildWars2.PvPCasterToolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CancellationTokenSource asyncPublisherCancellationTokenSource = new CancellationTokenSource();
        private IList<Task> asyncPublisherTasks = new List<Task>();

        protected override void OnStartup(StartupEventArgs e)
        {
            IServiceProvider serviceProvider = this.BuildServiceProvider();

            ILogger logger = serviceProvider.GetRequiredService<ILogger<App>>();
            AppDomain.CurrentDomain.UnhandledException += (s, ex) => logger.LogCritical(ex.ExceptionObject as Exception, string.Empty);
            this.DispatcherUnhandledException += (s, ex) => logger.LogCritical(ex.Exception, string.Empty);

            base.OnStartup(e);

            // start async publishers
            this.asyncPublisherTasks.Add(serviceProvider.GetRequiredService<Gw2MumbleLinkPublisher>()
                                                        .RunAsync(asyncPublisherCancellationTokenSource.Token));
            this.asyncPublisherTasks.Add(serviceProvider.GetRequiredService<Gw2ScreenshotPublisher>()
                                                        .RunAsync(asyncPublisherCancellationTokenSource.Token));

            var mainWindow = ActivatorUtilities.GetServiceOrCreateInstance<MainWindow>(serviceProvider);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // kill all async publishers
            this.asyncPublisherCancellationTokenSource.Cancel();
            try { Task.WaitAll(this.asyncPublisherTasks.ToArray(), TimeSpan.FromSeconds(5)); }
            catch (AggregateException aex) { aex.Handle(ex => ex is TaskCanceledException); }
            catch (TaskCanceledException) { }
            this.asyncPublisherTasks.Clear();

            base.OnExit(e);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            // configuration
            services.AddSingleton<AppConfig>();

            // logging services
            services.AddSingleton<ObservableCollectionLoggerProvider>();
            services.AddSingleton(new FileLoggerProvider("./log.txt"));
            services.AddSingleton<ILoggerFactory>(serviceProvider => new LoggerFactory(new ILoggerProvider[]
            {
                serviceProvider.GetRequiredService<ObservableCollectionLoggerProvider>(),
                serviceProvider.GetRequiredService<FileLoggerProvider>()
            }));
            services.AddLogging();

            // OCR
            services.AddSingleton(new TesseractEngine("./tessdata", "eng", EngineMode.Default, null, new Dictionary<string, object>
            {
                ["load_system_dawg"] = false,
                ["load_freq_dawg"] = false
            }, false));

            // async publishers
            services.AddSingleton<Gw2MumbleLinkPublisher>();
            services.AddSingleton<Gw2ScreenshotPublisher>();

            // game state manager
            services.AddSingleton<GameStateManager>();

            return services.BuildServiceProvider();
        }
    }
}
