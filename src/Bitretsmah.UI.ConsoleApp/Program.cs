using System;

namespace Bitretsmah.UI.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //args = new string[] { "-i" };
            //args = new string[] { "--set-account", "zbik@ttt.pl" };

            var handler = new ConsoleCommandsHandler();
            handler.Handle(args);

            Console.WriteLine("Bitretsmah finished!");
            Console.ReadKey();
        }
    }
}