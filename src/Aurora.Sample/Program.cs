using Aurora.Core.Infrastructure;

namespace Aurora.Sample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper<Module>();
            bootstrapper.Run(args);
        }
    }
}