namespace SpecialUsers.Workers;

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

        var mailManConfig = configuration.GetSection("MailMan.ConsumerConfig").Get<MailConsumerConfig>();
        services.AddMailMan(opt => opt.HavingDefaultConsumerConfiguration(mailManConfig!)
                .AddDeliver<WikiAccount, SpecialUser>(consumerA => consumerA
                    .ConsumingKafkaTopic("wiki-information-accounts")
                    .HavingConsumerGroup("special-users-workers")
                    .WithHandler(wikiAccount => new(wikiAccount.Name, Origin: SpecialUser.OriginEnum.Wiki))
                    .ProducingToKafkaTopic("output-special-users")
                    )
                .AddDeliver<ForumAccount, SpecialUser>(consumerB => consumerB
                    .ConsumingKafkaTopic("forum-information-accounts")
                    .HavingConsumerGroup("special-users-workers-forum")
                    .WithHandler(AsyncHandler)
                    .ProducingToKafkaTopic("output-special-users")
            )
        );
    }

    public async static Task<SpecialUser> AsyncHandler(ForumAccount forumAccount)
    {
        await Task.Delay(10);
        return new(forumAccount.Name, Origin: SpecialUser.OriginEnum.Forum);
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

