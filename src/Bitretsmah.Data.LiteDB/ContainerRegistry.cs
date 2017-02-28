using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Data.LiteDB
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<IAccountRepository>().Use<AccountRepository>();
            For<ITargetRepository>().Use<TargetRepository>();
            For<IBackupRepository>().Use<BackupRepository>();
        }
    }
}