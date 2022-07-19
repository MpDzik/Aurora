using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Aurora.Core.Commands;

namespace Aurora.Core.Infrastructure
{
    /// <summary>
    /// Marker interface for components which should be registered in the dependency injection container
    /// in transient (per-dependency) scope.
    /// </summary>
    public interface IDependency
    {
    }

    /// <summary>
    /// Marker interface for components which should be registered in the dependency injection container
    /// in singleton scope.
    /// </summary>
    public interface ISingletonDependency
    {
    }

    /// <summary>
    /// Base class for all Autofac modules.
    /// </summary>
    public abstract class Module : Autofac.Module
    {
        private readonly Assembly _assembly;

        protected Module()
        {
            _assembly = Assembly.GetCallingAssembly();
        }

        protected abstract void RegisterComponents(ContainerBuilder builder);

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assembly)
                .AssignableTo<IDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            builder.RegisterAssemblyTypes(_assembly)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterAssemblyTypes(_assembly)
                .AssignableTo<ICommand>()
                .Where(x => GetArgumentsType(x) != null)
                .Keyed<ICommand>(x => GetArgumentsType(x))
                .AsImplementedInterfaces()
                .SingleInstance();

            RegisterComponents(builder);
        }

        private static Type GetArgumentsType(Type commandType)
        {
            return commandType.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommand<>))
                .Select(x => x.GetGenericArguments().FirstOrDefault())
                .FirstOrDefault();
        }
    }
}