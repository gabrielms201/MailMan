namespace SpecialUsers.Workers;

using Confluent.Kafka;
using MailMan.Core.Consumers;
using MailMan.Installers.Extensions.DependencyInjection;
using SpecialUsers.Workers.Contracts;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        AddServices(builder.Services, builder.Configuration);


        var app = builder.Build();
        ConfigureApplication(app);

        app.Run();
    }

    static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddMailMan(opt => opt.HavingDefaultConsumerConfiguration(configuration.GetValue<MailConsumerConfig>("MailMan.ConsumerConfig")!)
                .AddDeliver<WikiAccount, SpecialUser>(consumerA => consumerA
                    .ConsumingKafkaTopic("wiki-information-accounts")
                    .HavingConsumerGroup("special-users-workers")
                        .WithHandler(wikiAccount => new(wikiAccount.Name, Origin: SpecialUser.OriginEnum.Wiki))
                    )
                .AddDeliver<ForumAccount, SpecialUser>(consumerB => consumerB
                    .ConsumingKafkaTopic("forum-information-accounts")
                    .HavingConsumerGroup("special-users-workers")
                        .WithHandler(AsyncHandler),
                kafkaConsumerConfig: MailConsumerConfig.FromConfluentConfig(new ConsumerConfig { AllowAutoCreateTopics = true })
            )
        );
    }

    public async static Task<SpecialUser> AsyncHandler(ForumAccount forumAccount)
    {
        await Task.Delay(10);
        return new(forumAccount.Name, Origin: SpecialUser.OriginEnum.Wiki);
    }

    static void ConfigureApplication(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}

