using Confluent.Kafka;
using System.Text.Json;

namespace MailMan.Core.Consumers
{
    public interface IMailConsumer<TInput> : IDisposable
    {
        public TInput ConsumeAndGetInput(CancellationToken cancellationToken);
    }

    //TODO: Provavelmente, o ideal seria criar uma nova classe que abrange o consumer e o producer. IMailDeliver.
    //No momento, está assim pois até agora era apenas uma POC
    public class MailConsumer<TInput> : IMailConsumer<TInput>, IDisposable
    {
        private string Topic { get; set; } = string.Empty;
        private string ConsumerGroup { get; set; } = string.Empty;

        private MailConsumerConfig ConsumerConfig { get; set; } = MailConsumerConfig.Empty;

        private IConsumer<string, byte[]> Consumer { get; set; }

        public MailConsumer(
            string topic,
            string consumerGroup,
            MailConsumerConfig consumerConfig)
        {
            //TODO: Criar validador de config
            Topic = topic;
            ConsumerGroup = consumerGroup;
            ConsumerConfig = consumerConfig;
            ConsumerConfig.KafkaConfig.GroupId = ConsumerGroup;


            Consumer = new ConsumerBuilder<string, byte[]>(ConsumerConfig.KafkaConfig).Build();
            Consumer.Subscribe(Topic);
        }

        public TInput ConsumeAndGetInput(CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            var message = Consumer.Consume(cancellationToken);

            //TODO: Adicionar suporte a open telemetry

            cancellationToken.ThrowIfCancellationRequested();
            var input = JsonSerializer.Deserialize<TInput>(message.Message.Value);

            if (input is null)
            {
                //TOOD: Log warn
                throw new InvalidOperationException("TODO:");
            }

            return input;
        }

        public MailConsumerConfig GetConsumerConfig() => ConsumerConfig;

        public string GetTopic() => Topic;

        public void Dispose()
        {
            //TODO: Fazer isso direito
            Consumer!.Dispose();
        }
    }
}
