using System;

namespace Bitretsmah.UI.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var handler = new ConsoleCommandsHandler();
            handler.Handle(args);

            Console.WriteLine("Bitretsmah finished!");
        }
    }
}