using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace GuildWars2.PvPCasterToolbox.Logging
{
    internal sealed class ObservableCollectionLogger : ILogger
    {
        private readonly ObservableCollection<string> logBuffer;

        public ObservableCollectionLogger(ObservableCollection<string> logBuffer)
            => this.logBuffer = logBuffer;

        public IDisposable BeginScope<TState>(TState state)
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                string formattedMessage = formatter?.Invoke(state, exception);

                if (exception != null)
                {
                    formattedMessage += $"\n{exception.GetType().Name}: {exception.Message}";
                }

                formattedMessage = formattedMessage?.Trim();

                if (!string.IsNullOrWhiteSpace(formattedMessage))
                {
                    foreach (var line in formattedMessage.Split('\n'))
                    {
                        this.logBuffer.Add(line);
                    }
                }
            }
        }
    }
}
