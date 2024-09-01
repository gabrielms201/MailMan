using MailMan.Core.Configurator;
using Microsoft.Extensions.DependencyInjection;

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

            return services;
        }
    }
}
