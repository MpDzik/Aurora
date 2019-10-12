using System;
using System.Linq;
using Aurora.Core.Commands;
using Autofac;
using CommandLine;
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
            var builder = new ContainerBuilder();
            builder.RegisterInstance(args).AsImplementedInterfaces();
            builder.RegisterModule<TModule>();

            var container = builder.Build();
            var command = container.ResolveKeyed<ICommand>(args.GetType());
            command.SetArguments((ICommandArguments)args);

            try
            {
                command.Execute();
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
    }
}