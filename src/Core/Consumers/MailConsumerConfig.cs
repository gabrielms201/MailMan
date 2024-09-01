using Confluent.Kafka;

namespace MailMan.Core.Consumers
{
    public class MailConsumerConfig
    {
        public static readonly MailConsumerConfig Empty = new();

        public ConsumerConfig KafkaConfig { get; set; } = null!;

        public static MailConsumerConfig FromConfluentConfig(ConsumerConfig kafkaConfig)
        {
            return new MailConsumerConfig(kafkaConfig);
        }
        private MailConsumerConfig(ConsumerConfig kafkaConfig)
        {
            KafkaConfig = kafkaConfig;
        }

        public MailConsumerConfig()
        {
        }
    }
}
