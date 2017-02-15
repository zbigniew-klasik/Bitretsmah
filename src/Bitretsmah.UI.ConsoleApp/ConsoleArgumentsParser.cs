using Fclp;
using System;
using System.Linq;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.UI.ConsoleApp
{
    internal interface IConsoleArgumentsParser
    {
        ConsoleArguments Parse(string[] args);
    }

    internal class ConsoleArgumentsParser : IConsoleArgumentsParser
    {
        private ILogger _logger;

        public ConsoleArgumentsParser(ILogger logger)
        {
            _logger = logger;
        }

        public ConsoleArguments Parse(string[] args)
        {
            var parser = new FluentCommandLineParser<ConsoleArguments>();

            parser.Setup(x => x.Accounts).As("accounts");
            parser.Setup(x => x.Backup).As("backup");
            parser.Setup(x => x.Forced).As("forced");
            parser.Setup(x => x.Help).As("help");
            parser.Setup(x => x.Password).As("password");
            parser.Setup(x => x.Path).As("path");
            parser.Setup(x => x.Restore).As("restore");
            parser.Setup(x => x.SetAccount).As("set-account");
            parser.Setup(x => x.SetTarget).As("set-target");
            parser.Setup(x => x.Targets).As("targets");
            parser.Setup(x => x.Version).As("version");

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                throw new ArgumentException(result.ErrorText);
            }

            if (result.AdditionalOptionsFound.Any())
            {
                throw new ArgumentException($"Unknown argument: {result.AdditionalOptionsFound.First().Key}.");
            }

            LogArguments(args, parser.Object);

            return parser.Object;
        }

        private void LogArguments(string[] inputArguments, ConsoleArguments parsedArguments)
        {
            var joinedArguments = string.Join(" ", inputArguments);

            if (parsedArguments.Password != null)
            {
                joinedArguments = joinedArguments.Replace(parsedArguments.Password, "**********");
            }

            _logger.Info("Run with arguments: {0}.", joinedArguments);
        }
    }
}