using StructureMap;

namespace Bitretsmah.Core
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<IAccountService>().Use<AccountService>();
            For<IBackupService>().Use<BackupService>();
            For<IChangedFilesUploader>().Use<ChangedFilesUploader>();
            For<IFileHashService>().Use<FileHashService>();
            For<IHistoryService>().Use<HistoryService>();
            For<INodeChangesApplier>().Use<NodeChangesApplier>();
            For<INodeChangesFinder>().Use<NodeChangesFinder>();
            For<IRemoteFileWarehouse>().Use<RemoteFileWarehouse>();
            For<IRemoteFileWarehouseFactory>().Use<RemoteFileWarehouseFactory>();
            For<ITargetService>().Use<TargetService>();
        }
    }
}