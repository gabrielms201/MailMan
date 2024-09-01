namespace MailMan.Core.Manager
{
    public interface IConsumerManager
    {
        public Task RunAsync(CancellationToken cancellation);
    }
}