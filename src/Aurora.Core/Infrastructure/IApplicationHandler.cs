using System;
using Aurora.Core.Commands;

namespace Aurora.Core.Infrastructure
{
    public interface IApplicationHandler : IDependency
    {
        int Priority { get; }
        void OnApplicationInitialize();
        void OnCommandInitialize(ICommand command);
        void OnCommandCompleted(ICommand command);
        void OnError(ICommand command, Exception exception);
    }
}