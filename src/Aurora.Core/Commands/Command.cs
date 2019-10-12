namespace Aurora.Core.Commands
{
    public interface ICommand
    {
        void Execute();
        void SetArguments(ICommandArguments arguments);
    }

    public interface ICommand<out TArgs> : ICommand
        where TArgs : ICommandArguments
    {
        TArgs Arguments { get; }
    }

    public abstract class Command<TArgs> : ICommand<TArgs>
        where TArgs : ICommandArguments
    {
        public abstract void Execute();

        public void SetArguments(ICommandArguments arguments)
        {
            Arguments = (TArgs) arguments;
        }

        public TArgs Arguments { get; private set; }
    }
}