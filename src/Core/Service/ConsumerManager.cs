using Confluent.Kafka;
using MailMan.Core.Configurator;
using MailMan.Core.Consumers;
using System.Collections.Concurrent;
using System.Text.Json;

namespace MailMan.Core.Manager
{
    public class ConsumerManager : IConsumerManager
    {
        private readonly IMailConfigurator _configurator;
        //TODO: Melhorar
        private readonly ConcurrentBag<Task> _tasks = [];

        public ConsumerManager(IMailConfigurator configurator)
        {
            _configurator = configurator;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var consumers = _configurator.GetConsumers();
                foreach (var consumer in consumers)
                {
                    _tasks.Add(Task.Run(() => StartConsumerThreadAsync(consumer, cancellationToken), cancellationToken));
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

        public static async Task StartConsumerThreadAsync(IMailConsumer mailConsumer, CancellationToken cancellationToken)
        {
            var inputType = mailConsumer.GetInputType();
            var config = mailConsumer.GetConsumerConfig().KafkaConfig;
            var topic = mailConsumer.GetTopic();

            using var kafkaConsumer = new ConsumerBuilder<string, string>(config).Build();
            kafkaConsumer.Subscribe(topic);
            //TODO: melhorar
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var message = kafkaConsumer.Consume(cancellationToken);
                    //TODO: Arrumar essa conversao horrivel de json
                    var messageAsObj = JsonSerializer.Deserialize(message.Message.Value, inputType);

                    await mailConsumer.ExecuteAsync(messageAsObj, cancellationToken);
                }
                catch (Exception ex)
                {
                    //todo: tratar
                    await Console.Out.WriteLineAsync(ex.ToString());
                }
            }
        }
    }
}
