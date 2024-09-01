using MailMan.Core.Configurator;
using System.Collections.Concurrent;

namespace MailMan.Core.Manager
{
    public class DeliveryManager : IDeliveryManager, IDisposable
    {
        private readonly IMailConfigurator _configurator;
        //TODO: Melhorar
        private readonly ConcurrentBag<Task> _tasks = [];

        public DeliveryManager(IMailConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var deliveries = _configurator.GetDeliveries();
                foreach (var delivery in deliveries)
                {
                    _tasks.Add(Task.Run(() => delivery.StartAsync(cancellationToken), cancellationToken));
                }

                await Task.WhenAll(_tasks);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
                //TODO: Tratar
                throw;
            }
            return;
        }
    }
}
