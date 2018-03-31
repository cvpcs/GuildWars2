using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace GuildWars2.PvPOcr
{
    public class ObservableCollectionLogger : ILogger
    {
        public ObservableCollection<string> Collection = new ObservableCollection<string>();

        public IDisposable BeginScope<TState>(TState state)
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
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
                    Collection.Add(line);
                }
            }
        }
    }
}
