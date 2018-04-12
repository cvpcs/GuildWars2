using Microsoft.Extensions.Logging;
using System.IO;

namespace GuildWars2.PvPCasterToolbox.Logging
{
    public sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly StreamWriter fileWriter;

        public FileLoggerProvider(string path, bool append = false)
            => this.fileWriter = new StreamWriter(path, append);

        public ILogger CreateLogger(string categoryName)
            => new FileLogger(categoryName, this.fileWriter);

        public void Dispose()
            => this.fileWriter.Dispose();
    }
}
