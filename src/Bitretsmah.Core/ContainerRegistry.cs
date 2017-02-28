using StructureMap;

namespace Bitretsmah.Core
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<IAccountService>().Use<AccountService>().Singleton(); ;
            For<IBackupService>().Use<BackupService>().Singleton();
            For<IChangedFilesUploader>().Use<ChangedFilesUploader>().Singleton();
            For<IFileHashService>().Use<FileHashService>().Singleton();
            For<IHistoryService>().Use<HistoryService>().Singleton();
            For<INodeChangesApplier>().Use<NodeChangesApplier>().Singleton();
            For<INodeChangesFinder>().Use<NodeChangesFinder>().Singleton();
            For<IRemoteFileWarehouse>().Use<RemoteFileWarehouse>().Singleton();
            For<IRemoteFileWarehouseFactory>().Use<RemoteFileWarehouseFactory>().Singleton();
            For<ITargetService>().Use<TargetService>().Singleton();
        }
    }
}