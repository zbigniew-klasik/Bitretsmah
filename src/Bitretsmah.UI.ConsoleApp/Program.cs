using System;
using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.UI.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("BITRETSMAH DEBUG MODE");
            Console.Write("provide start arguments: ");
            var argsLine = Console.ReadLine();
            args = !string.IsNullOrWhiteSpace(argsLine) ? argsLine.Split(' ') : new string[] { };
#endif

            try
            {
                var container = new Container(ContainerRegistry.CreateCompleteRegistry());
                var logger = container.GetInstance<ILogger>();
                logger.Info("Bitretsmah START");

                try
                {
                    var parser = container.GetInstance<IConsoleArgumentsParser>();
                    var executor = container.GetInstance<IExecutor>();
                    var arguments = parser.Parse(args);
                    executor.Execut(arguments).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("UNEXPECTED EXCEPTION: {0}.", ex);
                    logger.Error(ex);
                }

                logger.Info("Bitretsmah FINISHED");
            }
            catch (Exception ex)
            {
                Console.WriteLine("FATAL EXCEPTION: {0}.", ex);
            }

#if DEBUG
            Console.WriteLine("BITRETSMAH FINISHED");
            Console.ReadKey();
#endif
        }
    }
}