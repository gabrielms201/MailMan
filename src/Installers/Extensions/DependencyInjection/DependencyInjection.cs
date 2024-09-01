using MailMan.Core.Configurator;
using MailMan.Core.Manager;
using MailMan.Core.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MailMan.Installers.Extensions.DependencyInjection
{
    public static class ServiceCollectionInstaller
    {
        public static IServiceCollection AddMailMan(
            this IServiceCollection services,
            Action<MailConfigurator> configure = null)
        {
            var configurator = new MailConfigurator();
            configure?.Invoke(configurator);

            services.AddSingleton<IMailConfigurator>(configurator);
            services.AddSingleton<IDeliveryManager, DeliveryManager>();

            AddHostedService(services);
            return services;
        }

        static void AddHostedService(IServiceCollection collection)
        {
            collection.AddOptions();
            collection.AddHealthChecks();

            collection.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, MailManHostedService>());
        }
    }
}
