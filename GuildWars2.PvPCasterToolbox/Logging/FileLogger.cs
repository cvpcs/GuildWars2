using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace GuildWars2.PvPCasterToolbox.Logging
{
    internal sealed class FileLogger : ILogger
    {
        private readonly string categoryName;
        private readonly StreamWriter fileWriter;

        public FileLogger(string categoryName, StreamWriter fileWriter)
        {
            this.categoryName = categoryName;
            this.fileWriter = fileWriter;
        }

        public IDisposable BeginScope<TState>(TState state)
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                string formattedMessage = formatter?.Invoke(state, exception);

                if (exception != null)
                {
                    formattedMessage += $"\n{exception.ToString()}";
                }

                formattedMessage = formattedMessage?.Trim();

                if (!string.IsNullOrWhiteSpace(formattedMessage))
                {
                    this.fileWriter.WriteLine(formattedMessage);
                    this.fileWriter.Flush();
                }
            }
        }
    }
}
