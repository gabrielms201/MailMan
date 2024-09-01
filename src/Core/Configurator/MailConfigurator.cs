using MailMan.Core.Consumers;
using System.Collections.Concurrent;

namespace MailMan.Core.Configurator
{
    public interface IMailConfigurator
    {

    }
    public class MailConfigurator : IMailConfigurator
    {
        private ConcurrentBag<IMailConsumer> _consumers = [];
        private MailConsumerConfig _defaultConsumerConfig = MailConsumerConfig.Empty;

        public MailConfigurator HavingDefaultConsumerConfiguration(MailConsumerConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            _defaultConsumerConfig = config;
            return this;
        }

        public MailConfigurator AddDeliver<TInput, TOutput>(
            Action<MailConsumerConfigurator<TInput, TOutput>> config,
            MailConsumerConfig kafkaConsumerConfig = null)
        {
            ArgumentNullException.ThrowIfNull(config);

            var configurator = new MailConsumerConfigurator<TInput, TOutput>();
            config!.Invoke(configurator);

            kafkaConsumerConfig ??= _defaultConsumerConfig;
            _consumers.Add(configurator.Build(kafkaConsumerConfig));
            return this;

        }
    }
}
