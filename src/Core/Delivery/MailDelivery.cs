using MailMan.Core.Consumers;
using MailMan.Core.Producers;

namespace MailMan.Core.Delivery
{
    internal class MailDelivery<TInput, TOutput> : IMailDelivery, IDisposable
    {
        public MailDelivery(Func<TInput, Task<TOutput>> handler, IMailConsumer<TInput> consumer, IMailProducer<TOutput> producer)
        {
            Handler = handler;
            Consumer = consumer;
            Producer = producer;
        }

        private Func<TInput, Task<TOutput>> Handler { get; set; }
        private IMailConsumer<TInput> Consumer { get; set; }
        private IMailProducer<TOutput> Producer { get; set; }

        public void Dispose()
        {
            //TODO: Fazer isso direito
            Consumer.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var message = Consumer.ConsumeAndGetInput(cancellationToken);
                    var output = await Handler.Invoke(message);

                    await Producer.ProduceAsync(output, cancellationToken);
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
