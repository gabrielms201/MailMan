namespace MailMan.Core.Delivery
{
    public interface IMailDelivery
    {
        public Task StartAsync(CancellationToken cancellationToken);
    }
}
