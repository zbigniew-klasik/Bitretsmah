using Bitretsmah.Core.Exceptions;
using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Bitretsmah.UI.ConsoleApp
{
    public interface IConsoleService
    {
        void WriteHelp();

        void WriteVersion();

        SecureString ReadPassword();

        void AccountSetSuccessfully();

        void ListAccounts(IEnumerable<Account> accounts);

        void TargetSetSuccessfully();

        void ListTargets(IEnumerable<Target> targets);

        void WriteErrorMessage(string message);

        void WriteUnexpectedException(Exception exception);
    }

    internal class ConsoleService : IConsoleService
    {
        public const string ProgramName = "Bitretsmah";

        public void WriteHelp()
        {
            Console.WriteLine($"{ProgramName} usage:");
            Console.WriteLine("--backup target");
            Console.WriteLine("--restore target");
            Console.WriteLine("--set-account user [--password password]");
            Console.WriteLine("--set-target target --path path");
            Console.WriteLine("--forced");
            Console.WriteLine("--help");
            Console.WriteLine("--version");

            //TODO
        }

        public void WriteVersion()
        {
            var version = "unknown";

            var location = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrWhiteSpace(location))
            {
                version = FileVersionInfo.GetVersionInfo(location).FileVersion;
            }

            Console.WriteLine($"{ProgramName} version: {version}");
        }

        public SecureString ReadPassword()
        {
            Console.Write("Enter password: ");

            var pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
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

        public void AccountSetSuccessfully()
        {
            Console.WriteLine("Account configured successfully.");
        }

        public void ListAccounts(IEnumerable<Account> accounts)
        {
            var accountsList = accounts.ToList();

            if (accountsList.Any())
            {
                decimal totalQuota = accountsList.Sum(x => x.Quota.Total);
                decimal usedQuota = accountsList.Sum(x => x.Quota.Used);
                decimal freeQuota = accountsList.Sum(x => x.Quota.Free);

                Console.WriteLine("Accounts:");
                accountsList.ForEach(x => Console.WriteLine($"\t{x.Credential.UserName}\t{x.Quota.Free}"));
                Console.WriteLine($"Quota: {totalQuota}\tUsed: {usedQuota}\tFree: {freeQuota}");
            }
            else
            {
                Console.WriteLine("No accounts configured.");
            }
        }

        public void TargetSetSuccessfully()
        {
            Console.WriteLine("Target configured successfully.");
        }

        public void ListTargets(IEnumerable<Target> targets)
        {
            var targetsList = targets.ToList();

            if (targetsList.Any())
            {
                Console.WriteLine("Targets:");
                targetsList.ForEach(x => Console.WriteLine($"\t{x.Name}\t{x.LocalPath}"));
            }
            else
            {
                Console.WriteLine("No targets configured.");
            }
        }

        public void WriteErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void WriteUnexpectedException(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"UNEXPECTED EXCEPTION:");
            Console.WriteLine($"{exception.GetType()}:\t{exception.Message}");
            Console.WriteLine();
            Console.WriteLine($"SEE PROGRAM LOG FOR DETAILS:");
            Console.WriteLine(); // todo: WRITE LOG PATH
            Console.ResetColor();
        }
    }
}