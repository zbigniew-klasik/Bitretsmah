using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Data.System
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<IDateTimeService>().Use<DateTimeService>();
            For<IFileHashProvider>().Use<FileHashProvider>();
            For<ILocalFilesService>().Use<LocalFilesService>();
        }
    }
}