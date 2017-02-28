using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.UI.ConsoleApp
{
    public class ContainerRegistry : Registry
    {
        public static Registry CreateCompleteRegistry()
        {
            var registry = new Registry();
            registry.IncludeRegistry<Data.System.ContainerRegistry>();
            registry.IncludeRegistry<Data.LiteDB.ContainerRegistry>();
            registry.IncludeRegistry<Data.Mega.ContainerRegistry>();
            registry.IncludeRegistry<Core.ContainerRegistry>();
            registry.IncludeRegistry<ContainerRegistry>();
            return registry;
        }

        public ContainerRegistry()
        {
            For<IConsoleArgumentsParser>().Use<ConsoleArgumentsParser>();
            For<IConsoleService>().Use<ConsoleService>();
            For<IExecutor>().Use<Executor>();
            For<ILogger>().Use<Logger>();
        }
    }
}