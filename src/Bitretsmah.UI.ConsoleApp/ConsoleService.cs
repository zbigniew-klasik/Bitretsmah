using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;

namespace Bitretsmah.UI.ConsoleApp
{
    internal interface IConsoleService
    {
        void WriteHelp();

        void WriteVersion();

        SecureString ReadPassword();
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
    }
}