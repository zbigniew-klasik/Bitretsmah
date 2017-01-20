using StructureMap;

namespace Bitretsmah.UI.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new Container(new AppRegistry());
            var parser = container.GetInstance<IConsoleArgumentsParser>();
            var executor = container.GetInstance<IExecutor>();
            var arguments = parser.Parse(args);
            executor.Execut(arguments).Wait();
        }
    }
}