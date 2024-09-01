using MailMan.Core.Manager;
using Microsoft.Extensions.Hosting;

namespace MailMan.Core.Service
{
    public class MailManHostedService : IHostedService,
        IAsyncDisposable
    {
        public MailManHostedService(IConsumerManager consumerManager)
        {
            _consumerManager = consumerManager;
        }

        public IConsumerManager _consumerManager { get; private set; }

        public async ValueTask DisposeAsync()
        {
            await Task.Delay(1);
            return;
            //throw new NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _consumerManager.RunAsync(cancellationToken);
            await Console.Out.WriteLineAsync("aa");
            throw new NotImplementedException();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
