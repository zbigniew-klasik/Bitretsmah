using StructureMap;
using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Data.LiteDB;
using Bitretsmah.Data.Mega;
using Bitretsmah.Data.System;

namespace Bitretsmah.UI.ConsoleApp
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            // Bitretsmah.UI.ConsoleApp
            For<IConsoleArgumentsParser>().Use<ConsoleArgumentsParser>();
            For<IExecutor>().Use<Executor>();
            For<IConsoleService>().Use<ConsoleService>();

            // Bitretsmah.Core
            For<IAccountService>().Use<AccountService>();
            For<IBackupService>().Use<BackupService>();
            For<IChangedFilesUploader>().Use<ChangedFilesUploader>();
            For<IHistoryService>().Use<HistoryService>();
            For<INodeChangesApplier>().Use<NodeChangesApplier>();
            For<INodeChangesFinder>().Use<NodeChangesFinder>();
            For<IRemoteFileWarehouse>().Use<RemoteFileWarehouse>();
            For<IRemoteFileWarehouseFactory>().Use<RemoteFileWarehouseFactory>();
            For<ITargetService>().Use<TargetService>();

            // Bitretsmah.Data.LiteDB
            For<IAccountRepository>().Use<AccountRepository>();
            For<ITargetRepository>().Use<TargetRepository>();
            For<IBackupRepository>().Use<BackupRepository>();

            // Bitretsmah.Data.Mega
            For<ICredentialVerifier>().Use<MegaCredentialVerifier>();
            For<IRemoteFileStore>().Use<MegaStore>();
            For<IRemoteFileStoreFactory>().Use<MegaStoreFactory>();

            // Bitretsmah.Data.System
            For<IDateTimeService>().Use<DateTimeService>();
            For<IHashService>().Use<HashService>();
            For<ILocalFilesService>().Use<LocalFilesService>();
        }
    }
}