using MailMan.Core.Producers;
using System.Text.Json;

namespace MailMan.Core.Consumers
{
    public interface IMailConsumer
    {
        public Task ExecuteAsync(byte[] input, CancellationToken cancellationToken);
        public MailConsumerConfig GetConsumerConfig();
        public string GetTopic();
    }
    //TODO: Provavelmente, o ideal seria criar uma nova classe que abrange o consumer e o producer. IMailDeliver.
    //No momento, está assim pois até agora era apenas uma POC
    public class MailConsumer<TInput, TOutput> : IMailConsumer
    {
        private string Topic { get; set; } = string.Empty;
        private string ConsumerGroup { get; set; } = string.Empty;

        private Func<TInput, Task<TOutput>> Handler { get; set; }
        private MailConsumerConfig ConsumerConfig { get; set; } = MailConsumerConfig.Empty;
        private MailProducer<TOutput> Producer { get; set; } = MailProducer<TOutput>.Empty;

        public MailConsumer(
            string topic,
            string consumerGroup,
            Func<TInput, Task<TOutput>> handler,
            MailConsumerConfig consumerConfig,
            MailProducer<TOutput> producer = null!)
        {
            //TODO: Criar validador de config
            Topic = topic;
            ConsumerGroup = consumerGroup;
            ConsumerConfig = consumerConfig;
            ConsumerConfig.KafkaConfig.GroupId = ConsumerGroup;
            Handler = handler;

            producer ??= MailProducer<TOutput>.Empty;
            Producer = producer;
        }

        public async Task ExecuteAsync(byte[] inputBytes, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var input = JsonSerializer.Deserialize<TInput>(inputBytes);

            if (input is null)
            {
                //TOOD: Log warn
                return;
            }

            var output = await Handler.Invoke(input);

            if (!Producer.IsEmpty && output is not null)
            {

                //TODO: Logar caso dê algo errado.
                //TODO: Logar caso outptu seja nulo
                await Producer.ProduceAsync(output, cancellationToken);
            }
        }

        public MailConsumerConfig GetConsumerConfig() => ConsumerConfig;

        public string GetTopic() => Topic;

    }
}
