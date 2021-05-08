using System.Collections.Generic;

namespace Aurora.Core.Infrastructure
{
    public interface IStartupInfoProvider : IDependency
    {
        int Priority { get; }
        IEnumerable<string> Provide();
    }
}