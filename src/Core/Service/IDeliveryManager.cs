namespace MailMan.Core.Manager
{
    public interface IDeliveryManager : IDisposable
    {
        public Task RunAsync(CancellationToken cancellation);
    }
}