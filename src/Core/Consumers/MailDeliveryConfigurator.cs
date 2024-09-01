using MailMan.Core.Delivery;
using MailMan.Core.Producers;

namespace MailMan.Core.Consumers
{
    public class MailDeliveryConfigurator<TInput, TOutput>
    {
        private string ConsumingTopic { get; set; } = string.Empty;
        private string ConsumerGroup { get; set; } = string.Empty;
        public Func<TInput, Task<TOutput>> Handler { get; private set; } = null!;

        public MailProducerConfig ProducerConfig = MailProducerConfig.Empty;
        public MailConsumerConfig ConsumerConfig = MailConsumerConfig.Empty;

        public string ProducerTopic { get; private set; } = string.Empty;

        public MailDeliveryConfigurator<TInput, TOutput> ConsumingKafkaTopic(string topic)
        {
            ConsumingTopic = topic;
            return this;
        }

        public MailDeliveryConfigurator<TInput, TOutput> HavingConsumerGroup(string consumerGroup)
        {
            //TODO: melhorar para config user consumer group
            ConsumerGroup = consumerGroup;
            return this;
        }
        public MailDeliveryConfigurator<TInput, TOutput> WithHandler(Func<TInput, TOutput> handler)
        {
            // Confesso que no momento isso não está legal.
            // O Ideal MESMO seria separar de alguma maneira para que haja handlers async e não async,
            // a fim de não sair criando contexto caso seja utilizado um carteiro simples sem métodos async.
            //TODO: Ajustar
            Handler = new Func<TInput, Task<TOutput>>(input => Task.FromResult(handler(input)));
            return this;
        }

        public MailDeliveryConfigurator<TInput, TOutput> WithHandler(Func<TInput, Task<TOutput>> handler)
        {
            Handler = handler;
            return this;
        }

        public MailDeliveryConfigurator<TInput, TOutput> ProducingToKafkaTopic(string topic, MailProducerConfig producerConfig = null)
        {
            ProducerConfig = producerConfig is null ? MailProducerConfig.Empty : producerConfig;
            ProducerTopic = topic;
            return this;
        }

        public IMailDelivery Build(MailConsumerConfig config)
        {
            if (ProducerConfig.IsEmpty)
                ProducerConfig = MailProducerConfig.CreateFromConsumerConfig(config);
            ConsumerConfig = config;

            var producer = new MailProducer<TOutput>(ProducerTopic, ProducerConfig);
            var consumer = new MailConsumer<TInput>(topic: ConsumingTopic, consumerGroup: ConsumerGroup, consumerConfig: ConsumerConfig);

            return new MailDelivery<TInput, TOutput>(handler: Handler, consumer: consumer, producer: producer);
        }
    }
}
