using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GuildWars2.PvPCasterToolbox
{
    public abstract class AsyncDataPublisherBase<TData>
    {
        private static readonly TimeSpan LoopShortDelay = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan LoopLongDelay = TimeSpan.FromSeconds(5);

        public event Action<TData> DataAvailable;

        protected ILogger logger;
        private Task task;

        public AsyncDataPublisherBase(ILogger logger)
            => this.logger = logger;

        public Task RunAsync(CancellationToken token)
        {
            if (task != null && !task.IsCompleted)
            {
                throw new InvalidOperationException("Attempted to re-run async publisher that is already running");
            }

            this.task = Task.Factory.StartNew(async () =>
            {
                this.logger.LogInformation("Starting publisher loop");
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    bool success = false;

                    if (DataAvailable != null)
                    {
                        try
                        {
                            TData data = await GetDataAsync(token);

                            this.OnDataAvailable(data);

                            success = true;
                        }
                        catch(Exception exception)
                        {
                            this.logger.LogError(exception, string.Empty);
                        }
                    }
                    else
                    {
                        this.logger.LogTrace("No data publishing handlers configured, ignoring");
                    }

                    await Task.Delay(success ? LoopShortDelay : LoopLongDelay);
                }
            }, token);

            return this.task;
        }

        public static implicit operator Task(AsyncDataPublisherBase<TData> publisher)
            => publisher.task;

        protected abstract Task<TData> GetDataAsync(CancellationToken token);

        private void OnDataAvailable(TData data)
        {
            try
            {
                DataAvailable?.Invoke(data);
            }
            finally
            {
                // ensure the data is disposed if it is disposable
                var disposableData = data as IDisposable;
                if (disposableData != null)
                {
                    disposableData.Dispose();
                }
            }
        }
    }
}
