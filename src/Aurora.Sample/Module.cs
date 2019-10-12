using Autofac;

namespace Aurora.Sample
{
    public class Module : Core.Infrastructure.Module
    {
        protected override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterModule<Aurora.Core.Module>();
            builder.RegisterModule<Aurora.SampleModule.Module>();

            // TODO: Register additional modules here.
        }
    }
}