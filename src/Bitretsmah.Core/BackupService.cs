using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public interface IBackupService
    {
        Task Backup(BackupRequest request);
    }

    public class BackupService : IBackupService
    {
        private readonly IBackupRepository _backupRepository;
        private readonly IHashService _hashService;
        private readonly IHistoryService _historyService;
        private readonly ILocalFilesService _localFilesService;
        private readonly INodeChangesFinder _nodeChangesFinder;
        private readonly IRemoteFileWarehouseFactory _remoteFileWarehouseFactory;

        public BackupService(IBackupRepository backupRepository, IHashService hashService, IHistoryService historyService, ILocalFilesService localFilesService, INodeChangesFinder nodeChangesFinder, IRemoteFileWarehouseFactory remoteFileWarehouseFactory)
        {
            _backupRepository = backupRepository;
            _hashService = hashService;
            _historyService = historyService;
            _localFilesService = localFilesService;
            _nodeChangesFinder = nodeChangesFinder;
            _remoteFileWarehouseFactory = remoteFileWarehouseFactory;
        }

        public async Task Backup(BackupRequest request)
        {
            EnsureArg.IsNotNull(request, nameof(request));
            EnsureArg.IsNotNullOrWhiteSpace(request.Target, nameof(request.Target));
            EnsureArg.IsNotNullOrWhiteSpace(request.LocalPath, nameof(request.LocalPath));

            var currentStructure = _localFilesService.GetNodeStructure(request.LocalPath);

            if (request.ComputeHashForEachFile)
            {
                await ComputeHashesForAllFiles(currentStructure, request.Progress);
            }

            var previousStructure = await _historyService.GetLastStructure(request.Target);

            var structureChange = _nodeChangesFinder.Find(previousStructure, currentStructure);

            await UploadNewFiles(structureChange, request.Progress);

            await SaveBackup(structureChange, request.Progress);
        }

        private Task ComputeHashesForAllFiles(Node change, IProgress<BackupProgress> progress)
        {
            // todo: foreach
            _hashService.ComputeFileHash(change.AbsolutePath);

            // update progress
            progress.Report(new BackupProgress());

            return Task.Run(() => _hashService.ComputeFileHash(change.AbsolutePath));
        }

        private Task UploadNewFiles(Node change, IProgress<BackupProgress> progress)
        {
            using (var warehouse = _remoteFileWarehouseFactory.Create())
            {
                _hashService.ComputeFileHash(change.AbsolutePath);
                // upload all files in a loop
                // update progress
                progress.Report(new BackupProgress());
            }

            return null;
        }

        private async Task SaveBackup(Node change, IProgress<BackupProgress> progress)
        {
            // TODO:
            // create new backup entity
            // upload backup to Mega
            // save backup in local DB
            // calculate indexes and save to local DB

            await _backupRepository.Add(new Backup());
        }
    }
}