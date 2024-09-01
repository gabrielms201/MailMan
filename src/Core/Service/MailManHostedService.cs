using MailMan.Core.Manager;
using Microsoft.Extensions.Hosting;

namespace MailMan.Core.Service
{
    public class MailManHostedService : IHostedService,
        IAsyncDisposable
    {
        public MailManHostedService(IDeliveryManager consumerManager)
        {
            _deliveryManager = consumerManager;
        }

        public IDeliveryManager _deliveryManager { get; private set; }

        public async ValueTask DisposeAsync()
        {
            //TODO: Implementar o dispose corretamente
            _deliveryManager.Dispose();

            await Task.Delay(1);
            return;
            //throw new NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _deliveryManager.RunAsync(cancellationToken);
            throw new NotImplementedException();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _deliveryManager.Dispose();
        }
    }
}
