using System;
using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.UI.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var container = new Container(new AppRegistry());
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
        }
    }
}