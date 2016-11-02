using System;
using Bitretsmah.Core;
using Bitretsmah.Data.LiteDB;
using Bitretsmah.Data.Mega;

namespace Bitretsmah.UI.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //args = new string[] { "-i" };
            //args = new string[] { "--set-account", "zbik@ttt.pl" };

            var service = new ConsoleService(new AccountService(new AccountRepository(), new MegaCredentialVerifier()));
            service.HandleArguments(args);

            Console.WriteLine("Bitretsmah finished!");
            Console.ReadKey();
        }
    }
}