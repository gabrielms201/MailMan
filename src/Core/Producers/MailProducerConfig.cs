using Confluent.Kafka;
using MailMan.Core.Consumers;

namespace MailMan.Core.Producers
{
    public class MailProducerConfig
    {
        public static readonly MailProducerConfig Empty = new();
        public bool IsNullOrEmpty => ReferenceEquals(this, Empty) || this is null;
        protected MailProducerConfig() { }

        public ProducerConfig KafkaConfig { get; private set; } = null!;

        public static MailProducerConfig FromConfluentConfig(ProducerConfig kafkaConfig)
        {
            return new MailProducerConfig(kafkaConfig);
        }
        private MailProducerConfig(ProducerConfig kafkaConfig)
        {
            KafkaConfig = kafkaConfig;
        }
        public static MailProducerConfig CreateFromConsumerConfig(MailConsumerConfig consumerConfig)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = consumerConfig.KafkaConfig.BootstrapServers,
                SecurityProtocol = consumerConfig.KafkaConfig.SecurityProtocol,
                SaslMechanism = consumerConfig.KafkaConfig.SaslMechanism,
                SaslUsername = consumerConfig.KafkaConfig.SaslUsername,
                SaslPassword = consumerConfig.KafkaConfig.SaslPassword,
                SslCaLocation = consumerConfig.KafkaConfig.SslCaLocation,
                SslCertificateLocation = consumerConfig.KafkaConfig.SslCertificateLocation,
                SslKeyLocation = consumerConfig.KafkaConfig.SslKeyLocation,
                SslKeyPassword = consumerConfig.KafkaConfig.SslKeyPassword,
                SslKeystoreLocation = consumerConfig.KafkaConfig.SslKeystoreLocation,
                SslKeystorePassword = consumerConfig.KafkaConfig.SslKeystorePassword,
                SslCrlLocation = consumerConfig.KafkaConfig.SslCrlLocation,
                SslCipherSuites = consumerConfig.KafkaConfig.SslCipherSuites,
                SslCurvesList = consumerConfig.KafkaConfig.SslCurvesList,
                SslSigalgsList = consumerConfig.KafkaConfig.SslSigalgsList,
                SslEndpointIdentificationAlgorithm = consumerConfig.KafkaConfig.SslEndpointIdentificationAlgorithm,
            };
            return FromConfluentConfig(producerConfig);
        }

    }
}
