namespace MailMan.Core.Producers
{
    public interface IMailProducer<TOutput>
    {
        public Task ProduceAsync(TOutput output);
    }
    public class MailProducer<TOutput> : IMailProducer<TOutput>
    {
        protected MailProducer() { }
        public MailProducer(MailProducerConfig config)
        {
            Config = config;
        }

        public static MailProducer<TOutput> Empty => new();
        public bool IsNullOrEmpty => ReferenceEquals(this, Empty) || this is null;
        public MailProducerConfig Config { get; private set; } = MailProducerConfig.Empty;

        public async Task ProduceAsync(TOutput output)
        {
            await Task.Delay(1);
            throw new NotImplementedException();
        }
    }
}
