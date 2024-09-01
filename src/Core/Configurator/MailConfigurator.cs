using MailMan.Core.Consumers;
using MailMan.Core.Delivery;
using System.Collections.Concurrent;

namespace MailMan.Core.Configurator
{
    public interface IMailConfigurator
    {
        public ConcurrentBag<IMailDelivery> GetDeliveries();
    }
    public class MailConfigurator : IMailConfigurator
    {
        private ConcurrentBag<IMailDelivery> _deliveries = [];
        private MailConsumerConfig _defaultConsumerConfig = MailConsumerConfig.Empty;

        public MailConfigurator HavingDefaultConsumerConfiguration(MailConsumerConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            _defaultConsumerConfig = config;
            return this;
        }

        public MailConfigurator AddDeliver<TInput, TOutput>(
            Action<MailDeliveryConfigurator<TInput, TOutput>> config,
            MailConsumerConfig kafkaConsumerConfig = null)
        {
            ArgumentNullException.ThrowIfNull(config);

            var configurator = new MailDeliveryConfigurator<TInput, TOutput>();
            config!.Invoke(configurator);

            kafkaConsumerConfig ??= _defaultConsumerConfig;
            _deliveries.Add(configurator.Build(kafkaConsumerConfig));
            return this;

        }

        public ConcurrentBag<IMailDelivery> GetDeliveries() => _deliveries;
    }
}
