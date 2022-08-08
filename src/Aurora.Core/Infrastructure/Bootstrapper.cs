using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aurora.Core.Commands;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Aurora.Core.Infrastructure
{
    public class Bootstrapper<TModule>
        where TModule : Module, new()
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Run(string[] args)
        {
            Parser.Default.ParseArguments(args, GetCommandTypes()).WithParsed(Run);
        }

        protected virtual Type[] GetCommandTypes()
        {
            // HACK: Build container to force loading all assemblies. This needs to be refactored.

            var builder = new ContainerBuilder();
            builder.RegisterModule<TModule>();
            builder.Build();

            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(x => !x.IsAbstract && x.IsAssignableTo<ICommandArguments>()))
                .ToArray();

            return commandTypes;
        }

        protected virtual void Run(object args)
        {
            var hostBuilder = CreateHostBuilder(args);

            try
            {
                var host = hostBuilder.Build();
                var rootScope = host.Services.GetAutofacRoot();

                var infoProviders = rootScope.Resolve<IEnumerable<IStartupInfoProvider>>();
                InvokeStartupInfoProviders(infoProviders);

                var command = rootScope.ResolveKeyed<ICommand>(args.GetType());
                command.SetArguments((ICommandArguments)args);

                var cts = new CancellationTokenSource();
                var backgroundService = (BackgroundService)command;
                backgroundService.StartAsync(cts.Token);
                backgroundService.ExecuteTask.Wait(cts.Token);
            }
            catch (ApplicationException ex)
            {
                Logger.Fatal(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        private static IHostBuilder CreateHostBuilder(object args)
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.UseServiceProviderFactory(
                new AutofacServiceProviderFactory(ContainerBuildOptions.None, builder => {
                    builder.RegisterInstance(args).AsImplementedInterfaces();
                    builder.RegisterModule<TModule>();
                }));

            using var container = BuildStartupContainer();
            var hostConfigurator = container.Resolve<IHostConfigurator>();
            hostConfigurator.Context = new HostConfiguratorContext(args);
            hostConfigurator.ConfigureBuilder(hostBuilder);
            hostBuilder.ConfigureServices(hostConfigurator.ConfigureServices);
            hostBuilder.ConfigureAppConfiguration(hostConfigurator.ConfigureAppConfiguration);
            hostBuilder.ConfigureHostConfiguration(hostConfigurator.ConfigureHostConfiguration);
            hostConfigurator.ConfigureHost(hostBuilder);

            return hostBuilder;
        }

        private static IContainer BuildStartupContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<TModule>();

            return builder.Build();
        }

        private static void InvokeStartupInfoProviders(IEnumerable<IStartupInfoProvider> providers)
        {
            foreach (var infoProvider in providers.OrderBy(x => x.Priority))
            {
                foreach (var infoText in infoProvider.Provide())
                {
                    Logger.Info(infoText);
                }
            }

            Console.WriteLine();
        } 
    }
}