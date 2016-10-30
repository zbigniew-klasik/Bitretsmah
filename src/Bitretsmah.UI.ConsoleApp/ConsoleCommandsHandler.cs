using Fclp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Bitretsmah.UI.ConsoleApp
{
    public class ConsoleCommandsHandler
    {
        private ConsoleCommands _commands;

        public void Handle(string[] args)
        {
            if (!ParseArguments(args)) return;

            HandleVersionShow();
            HandleAccountSet();
            HandleDirectorySet();
        }

        private bool ParseArguments(string[] args)
        {
            var parser = new FluentCommandLineParser<ConsoleCommands>();

            parser.SetupHelp("h", "help").Callback(ShowHelp).UseForEmptyArgs();
            parser.Setup(x => x.ShowVersion).As('v', "version").WithDescription("Shows the version of that application.");
            parser.Setup(x => x.SetAccount).As("set-account").WithDescription("Configures a storage account.");
            parser.Setup(x => x.SetDirectory).As("set-directory").WithDescription("Configures a directory to backup.");
            parser.Setup(x => x.Silent).As('s', "silent").WithDescription("Runs with the silent mode.");
            parser.Setup(x => x.Force).As('F', "forced").WithDescription("Runs with the forced mode."); ;

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                Console.WriteLine(result.ErrorText);
                return false;
            }

            if (result.AdditionalOptionsFound.Any())
            {
                result.AdditionalOptionsFound.ToList().ForEach(x => Console.WriteLine($"Unknown option: {x.Key}"));
                return false;
            }

            _commands = parser.Object;
            return true;
        }

        private void ShowHelp(string posibleCommands)
        {
            Console.WriteLine("Bitretsmah usage:");
            Console.WriteLine(posibleCommands);
        }

        private void HandleVersionShow()
        {
            if (!_commands.ShowVersion) return;
            Console.WriteLine($"Bitretsmah version: {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
        }

        private void HandleAccountSet()
        {
            if (_commands.SetAccount == null) return;

            Console.WriteLine("HandleAccountSet");
            _commands.SetAccount.ForEach(Console.WriteLine);
        }

        private void HandleDirectorySet()
        {
            if (_commands.SetDirectory == null) return;

            Console.WriteLine("HandleDirectorySet");
            Console.WriteLine(_commands.SetDirectory);
        }
    }
}