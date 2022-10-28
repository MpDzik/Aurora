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
            // HACK:
            // We need to build the boostrap container before calling GetCommandTypes to force loading all assemblies
            // into the current AppDomain. This may need to be refactored in the future, for now it works properly...
            var container = BuildBootstrapContainer();
            Parser.Default.ParseArguments(args, GetCommandTypes()).WithParsed(a => Run(a, container));
        }

        protected virtual Type[] GetCommandTypes()
        {
            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(x => !x.IsAbstract && x.IsAssignableTo<ICommandArguments>()))
                .ToArray();

            return commandTypes;
        }

        protected virtual void Run(object args, IContainer bootstrapContainer)
        {
            var hostBuilder = CreateHostBuilder(args, bootstrapContainer);
            var handlers = new List<IApplicationHandler>();
            var command = null as ICommand;

            try
            {
                var host = hostBuilder.Build();
                bootstrapContainer.Dispose();

                var rootScope = host.Services.GetAutofacRoot();

                handlers = rootScope.Resolve<IEnumerable<IApplicationHandler>>().ToList();
                InvokeHandlers(handlers, x => x.OnApplicationInitialize());

                var infoProviders = rootScope.Resolve<IEnumerable<IStartupInfoProvider>>();
                InvokeStartupInfoProviders(infoProviders);

                command = rootScope.ResolveKeyed<ICommand>(args.GetType());
                command.SetArguments((ICommandArguments)args);
                InvokeHandlers(handlers, x => x.OnCommandInitialize(command));

                var cts = new CancellationTokenSource();
                var backgroundService = (BackgroundService)command;
                backgroundService.StartAsync(cts.Token);
                backgroundService.ExecuteTask.Wait(cts.Token);
                InvokeHandlers(handlers, x => x.OnCommandCompleted(command));
            }
            catch (ApplicationException ex)
            {
                Logger.Fatal(ex.Message);
                InvokeHandlers(handlers, x => x.OnCommandCompleted(command));
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                InvokeHandlers(handlers, x => x.OnCommandCompleted(command));
            }
        }

        private static IHostBuilder CreateHostBuilder(object args, IContainer startupContainer)
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.UseServiceProviderFactory(
                new AutofacServiceProviderFactory(ContainerBuildOptions.None, builder => {
                    builder.RegisterInstance(args).AsImplementedInterfaces();
                    builder.RegisterModule<TModule>();
                }));

            var hostConfigurators = startupContainer.Resolve<IEnumerable<IHostConfigurator>>().ToList();
            var context = new HostConfiguratorContext(args);
            
            foreach (var hostConfigurator in hostConfigurators.OrderByDescending(x => x.Priority))
            {
                hostConfigurator.Context = context;
                hostConfigurator.ConfigureBuilder(hostBuilder);
                hostBuilder.ConfigureServices(hostConfigurator.ConfigureServices);
                hostBuilder.ConfigureAppConfiguration(hostConfigurator.ConfigureAppConfiguration);
                hostBuilder.ConfigureHostConfiguration(hostConfigurator.ConfigureHostConfiguration);
                hostConfigurator.ConfigureHost(hostBuilder);
            }

            return hostBuilder;
        }

        private static IContainer BuildBootstrapContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<TModule>();

            return builder.Build();
        }

        private static void InvokeHandlers(IEnumerable<IApplicationHandler> handlers, Action<IApplicationHandler> action)
        {
            foreach (var initializer in handlers.OrderBy(x => x.Priority))
            {
                action(initializer);
            }
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