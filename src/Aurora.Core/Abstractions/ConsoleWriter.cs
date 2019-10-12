using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Aurora.Core.Infrastructure;

namespace Aurora.Core.Abstractions
{
    public interface IConsoleWriter : ISingletonDependency
    {
        void Write(string value);
        void WriteLine();
        void WriteLine(string value);
    }

    [ExcludeFromCodeCoverage]
    public class ConsoleWriter : IConsoleWriter
    {
        private readonly TextWriter _writer;

        public ConsoleWriter()
        {
            _writer = Console.Out;
        }

        public void Write(string value)
        {
            _writer.Write(value);
        }

        public void WriteLine()
        {
            _writer.WriteLine();
        }

        public void WriteLine(string value)
        {
            _writer.WriteLine(value);
        }
    }
}