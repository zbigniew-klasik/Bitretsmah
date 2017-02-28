using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Data.System
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<IDateTimeService>().Use<DateTimeService>().Singleton();
            For<IFileHashProvider>().Use<FileHashProvider>().Singleton();
            For<ILocalFilesService>().Use<LocalFilesService>().Singleton();
        }
    }
}