using Microsoft.Extensions.Hosting;

namespace Aurora.Core.Commands
{
    public interface ICommand
    {
        void SetArguments(ICommandArguments arguments);
    }

    public interface ICommand<out TArgs> : ICommand
        where TArgs : ICommandArguments
    {
        TArgs Arguments { get; }
    }

    public abstract class Command<TArgs> : BackgroundService, ICommand<TArgs>
        where TArgs : ICommandArguments
    {
        public TArgs Arguments { get; private set; }

        public void SetArguments(ICommandArguments arguments)
        {
            Arguments = (TArgs) arguments;
        }
    }
}