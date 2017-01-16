using Bitretsmah.Core;
using Fclp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;

namespace Bitretsmah.UI.ConsoleApp
{
    public interface IConsoleService
    {
        void HandleArguments(string[] args);
    }

    public class ConsoleService : IConsoleService
    {
        private readonly IAccountService _accountService;
        private ConsoleArguments _commands;

        public ConsoleService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public void HandleArguments(string[] args)
        {
            if (!ParseArguments(args)) return;

            HandleVersionShow();
            HandleInfoShow();
            HandleAccountSet();
            HandleDirectorySet();
        }

        private bool ParseArguments(string[] args)
        {
            var parser = new FluentCommandLineParser<ConsoleArguments>();

            parser.SetupHelp("h", "help").Callback(ShowHelp).UseForEmptyArgs();
            parser.Setup(x => x.ShowVersion).As('v', "version").WithDescription("Shows the version of that application.");
            parser.Setup(x => x.ShowInfo).As('i', "info").WithDescription("Shows information about accounts and directories.");
            parser.Setup(x => x.SetAccountOld).As("set-account").WithDescription("Configures a storage account.");
            parser.Setup(x => x.SetDirectory).As("set-directory").WithDescription("Configures a directory to backup.");
            parser.Setup(x => x.Forced).As('F', "forced").WithDescription("Runs with the forced mode."); ;

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

        private void HandleInfoShow()
        {
            if (_commands.ShowInfo == false) return;

            try
            {
                var accounts = _accountService.GetAll().Result;
                accounts.ToList().ForEach(x => Console.WriteLine($"{x.Credential.UserName} {x.Quota.Used} {x.Quota.Total}"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                // TODO: log
            }
        }

        private void HandleAccountSet()
        {
            if (_commands.SetAccountOld == null) return;

            if (_commands.SetAccountOld.Count < 1 || _commands.SetAccountOld.Count > 2)
            {
                Console.Error.WriteLine("Invalid number of arguments.");
                return;
            }

            var credential = new NetworkCredential { UserName = _commands.SetAccountOld[0] };

            if (_commands.SetAccountOld.Count == 2)
            {
                credential.Password = _commands.SetAccountOld[1];
            }
            else
            {
                Console.Write("Password: ");
                credential.SecurePassword = GetPassword();
            }

            try
            {
                _accountService.SetCredential(credential).Wait();
                Console.WriteLine($"Account '{credential.UserName}' has been configured successfully.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                // TODO: log
            }
        }

        public SecureString GetPassword()
        {
            var pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd;
        }

        private void HandleDirectorySet()
        {
            if (_commands.SetDirectory == null) return;

            Console.WriteLine("HandleDirectorySet");
            Console.WriteLine(_commands.SetDirectory);
        }
    }
}