using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Aurora.Core.Infrastructure;

namespace Aurora.Core.Abstractions
{
    public interface IConsoleReader : ISingletonDependency
    {
        string ReadLine();
    }

    [ExcludeFromCodeCoverage]
    public class ConsoleReader : IConsoleReader
    {
        private readonly TextReader _reader;

        public ConsoleReader()
        {
            _reader = Console.In;
        }

        public string ReadLine()
        {
            return _reader.ReadLine();
        }
    }
}