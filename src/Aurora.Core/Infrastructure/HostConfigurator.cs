using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aurora.Core.Infrastructure
{
    public class HostConfiguratorContext
    {
        public HostConfiguratorContext(object args)
        {
            Args = args;
        }

        public object Args { get; }
    }

    public interface IHostConfigurator : IDependency
    {
        HostConfiguratorContext Context { get; set; }
        void ConfigureBuilder(IHostBuilder builder);
        void ConfigureServices(HostBuilderContext context, IServiceCollection services);
        void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder);
        void ConfigureHostConfiguration(IConfigurationBuilder builder);
        void ConfigureHost(IHostBuilder builder);
    }

    public class HostConfigurator : IHostConfigurator
    {
        public HostConfiguratorContext Context { get; set; }

        public virtual void ConfigureBuilder(IHostBuilder builder)
        {
        }

        public virtual void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
        }

        public virtual void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
        }

        public virtual void ConfigureHostConfiguration(IConfigurationBuilder builder)
        {
        }

        public virtual void ConfigureHost(IHostBuilder builder)
        {
        }
    }
}