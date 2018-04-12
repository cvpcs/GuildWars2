using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Threading;
using GuildWars2.PvPCasterToolbox.Logging;

namespace GuildWars2.PvPCasterToolbox.TabPages
{
    public partial class ConsoleLog : TabItem
    {
        public ObservableCollection<string> ConsoleOutput { get; } = new ObservableCollection<string>();

        public ConsoleLog(ObservableCollectionLoggerProvider loggerProvider)
        {
            InitializeComponent();

            loggerProvider.LogBuffer.CollectionChanged += (s, e) => Dispatcher.Invoke(() =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (object item in e.NewItems)
                    {
                        ConsoleOutput.Add(item.ToString());
                    }

                    ConsoleScroller.ScrollToBottom();
                }
            });
        }
    }
}
