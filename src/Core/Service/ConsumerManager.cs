using Confluent.Kafka;
using MailMan.Core.Configurator;
using MailMan.Core.Consumers;
using System.Collections.Concurrent;

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
            var config = mailConsumer.GetConsumerConfig().KafkaConfig;
            var topic = mailConsumer.GetTopic();

            using var kafkaConsumer = new ConsumerBuilder<string, byte[]>(config).Build();
            kafkaConsumer.Subscribe(topic);
            //TODO: melhorar
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var message = kafkaConsumer.Consume(cancellationToken);

                    //TODO: Adicionar suporte a open telemetry
                    await mailConsumer.ExecuteAsync(message.Message.Value, cancellationToken);
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
