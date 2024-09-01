using MailMan.Core.Producers;

namespace MailMan.Core.Consumers
{
    public class MailConsumerConfigurator<TInput, TOutput>
    {
        private string Topic { get; set; } = string.Empty;
        private string ConsumerGroup { get; set; } = string.Empty;
        public Func<TInput, Task<TOutput>> Handler { get; private set; } = null!;

        public MailProducerConfig ProducerConfig = MailProducerConfig.Empty;

        public MailConsumerConfigurator<TInput, TOutput> ConsumingKafkaTopic(string topic)
        {
            Topic = topic;
            return this;
        }

        public MailConsumerConfigurator<TInput, TOutput> HavingConsumerGroup(string consumerGroup)
        {
            ConsumerGroup = consumerGroup;
            return this;
        }
        public MailConsumerConfigurator<TInput, TOutput> WithHandler(Func<TInput, TOutput> handler)
        {
            Handler = new Func<TInput, Task<TOutput>>(input => Task.FromResult(handler(input)));
            return this;
        }

        public MailConsumerConfigurator<TInput, TOutput> WithHandler(Func<TInput, Task<TOutput>> handler)
        {
            Handler = handler;
            return this;
        }

        public MailConsumerConfigurator<TInput, TOutput> ProducingToKafkaTopic(string topic, MailProducerConfig producerConfig = null)
        {
            ProducerConfig = producerConfig;
            return this;
        }

        public IMailConsumer Build(MailConsumerConfig config)
        {
            if (ProducerConfig.IsNullOrEmpty)
                ProducerConfig = MailProducerConfig.CreateFromConsumerConfig(config);

            var producer = new MailProducer<TOutput>(ProducerConfig);

            //TODO: Ajustar para multi contrato
            //TODO: Ajustar definição de header
            return new MailConsumer<TInput, TOutput>(
                topic: Topic,
                consumerGroup: ConsumerGroup,
                handler: Handler,
                consumerConfig: config,
                producer: producer);
        }
    }
}
