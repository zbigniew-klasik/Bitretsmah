using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Data.LiteDB
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<IAccountRepository>().Use<AccountRepository>().Singleton();
            For<ITargetRepository>().Use<TargetRepository>().Singleton();
            For<IBackupRepository>().Use<BackupRepository>().Singleton();
        }
    }
}