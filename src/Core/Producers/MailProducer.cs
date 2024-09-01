using Confluent.Kafka;
using System.Text.Json;

namespace MailMan.Core.Producers
{
    public interface IMailProducer<TOutput>
    {
        public Task ProduceAsync(TOutput output, CancellationToken cancellationToken);
    }
    public class MailProducer<TOutput> : IMailProducer<TOutput>
    {
        protected MailProducer() { }
        public string Topic = string.Empty;
        public MailProducer(string topic, MailProducerConfig config)
        {
            Topic = topic;
            Config = config;
        }

        public static MailProducer<TOutput> Empty => new();
        public bool IsEmpty => ReferenceEquals(this, Empty);
        public MailProducerConfig Config { get; private set; } = MailProducerConfig.Empty;

        public async Task ProduceAsync(TOutput output, CancellationToken cancellationToken)
        {
            //TODO: Configurar partition Key
            if (string.IsNullOrEmpty(Topic))
            {
                //TODO: Log Warn
            }
            cancellationToken.ThrowIfCancellationRequested();

            var messageAsJson = JsonSerializer.Serialize(output);
            var message = new Message<string, string> { Value = messageAsJson };

            using var producer = new ProducerBuilder<string, string>(Config.KafkaConfig).Build();

            await producer.ProduceAsync(Topic, message, cancellationToken);
        }
    }
}
