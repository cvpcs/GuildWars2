using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GuildWars2.PvPCasterToolbox
{
    public abstract class AsyncProcessorBase
    {
        protected ILogger logger;
        private Task task;

        public AsyncProcessorBase(ILogger logger)
            => this.logger = logger;

        public Task RunAsync(CancellationToken token)
        {
            if (task != null && !task.IsCompleted)
            {
                throw new InvalidOperationException("Attempted to re-run processor that is already running");
            }

            this.task = Task.Factory.StartNew(async () =>
            {
                this.logger.LogInformation("Starting process loop");
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    await this.ProcessIterationAsync(token);
                }
            }, token);

            return this.task;
        }

        protected abstract Task ProcessIterationAsync(CancellationToken token);

        public static implicit operator Task(AsyncProcessorBase asyncProcessor)
            => asyncProcessor.task;
    }
}
