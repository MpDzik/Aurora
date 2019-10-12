using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Aurora.Core.Infrastructure;

namespace Aurora.Core.Abstractions
{
    public interface ISleeper : IDependency
    {
        void Sleep(TimeSpan timeSpan);
    }

    [ExcludeFromCodeCoverage]
    public class Sleeper : ISleeper
    {
        public void Sleep(TimeSpan timeSpan)
        {
            Thread.Sleep(timeSpan);
        }
    }
}