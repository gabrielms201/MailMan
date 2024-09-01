using MailMan.Core.Producers;

namespace MailMan.Core.Consumers
{
    public interface IMailConsumer
    {
        //TODO: Validar se é possível remover o object, e deixar como generics
        public Task ExecuteAsync(object input);
    }
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
            Handler = handler;
            ConsumerConfig = consumerConfig;

            producer ??= MailProducer<TOutput>.Empty;
            Producer = producer;
        }

        public async Task ExecuteAsync(object input)
        {
            if (input is not TInput)
            {
                throw new ArgumentException($"Input type is not {typeof(TInput)}");
            }

            var output = await Handler.Invoke((TInput)input);

            if (!Producer.IsNullOrEmpty && output is not null)
            {
                //TODO: Logar caso dê algo errado.
                //TODO: Logar caso outptu seja nulo
                await Producer.ProduceAsync(output);
            }
        }
    }
}
