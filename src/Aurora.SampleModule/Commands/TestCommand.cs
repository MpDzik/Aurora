using CommandLine;
using Aurora.Core.Abstractions;
using Aurora.Core.Commands;

namespace Aurora.SampleModule.Commands
{
    [Verb("hello", HelpText = "Says hello")]
    public class TestCommandArguments : CommandArguments
    {
    }

    public class TestCommand : Command<TestCommandArguments>
    {
        private readonly IConsoleWriter _writer;

        public TestCommand(IConsoleWriter writer)
        {
            _writer = writer;
        }

        public override void Execute()
        {
            _writer.WriteLine("Hello World!");
        }
    }
}