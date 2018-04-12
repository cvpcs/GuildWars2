using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace GuildWars2.PvPCasterToolbox.Logging
{
    public sealed class ObservableCollectionLoggerProvider : ILoggerProvider
    {
        public readonly ObservableCollection<string> LogBuffer = new ObservableCollection<string>();

        public ILogger CreateLogger(string categoryName)
            => new ObservableCollectionLogger(LogBuffer);

        public void Dispose()
        { }
    }
}
