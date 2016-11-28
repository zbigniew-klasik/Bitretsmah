using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public class BackupService
    {
        private readonly IBackupRepository _backupRepository;
        private readonly IHashService _hashService;
        private readonly ILocalFilesService _localFilesService;
        private readonly INodeChangesFinder _nodeChangesFinder;
        private readonly IRemoteFileWarehouseFactory _remoteFileWarehouseFactory;

        public BackupService(IBackupRepository backupRepository, IHashService hashService, ILocalFilesService localFilesService, INodeChangesFinder nodeChangesFinder, IRemoteFileWarehouseFactory remoteFileWarehouseFactory)
        {
            _backupRepository = backupRepository;
            _hashService = hashService;
            _localFilesService = localFilesService;
            _nodeChangesFinder = nodeChangesFinder;
            _remoteFileWarehouseFactory = remoteFileWarehouseFactory;
        }

        public async Task CreateBackup(BackupRequest request)
        {
            var current = GetCurrentLocalStructure(request.LocalPath);

            if (request.ComputeHashForEachFile)
            {
                await ComputeHashesForAllFiles(null, null);
            }

            var last = await GetLastKnownStructure(request);

            var change = FindChange();

            if (!request.ComputeHashForEachFile)
            {
                await ComputeHashesForNewFiles(change, request.Progress);
            }

            await UploadNewFiles(change, request.Progress);

            await SaveBackup(change);

            throw new NotImplementedException();
        }

        private Node GetCurrentLocalStructure(string path)
        {
            // if path exists:
            return _localFilesService.GetNodeStructure(path);
        }

        private async Task<Node> GetLastKnownStructure(BackupRequest request)
        {
            // get local structure

            // get last time data
            var lastBackup = await _backupRepository.GetLastForTarget(request.Targer);

            Node change;

            if (lastBackup == null)
            {
                // to do mark all as added
                change = null;
            }
            else
            {
                var previousStructure = lastBackup.Change;
                change = _nodeChangesFinder.Find(previousStructure, null);
            }

            return change;
        }

        private Node FindChange()
        {
            return null;
        }

        private Task ComputeHashesForAllFiles(Node change, IProgress<BackupProgress> progress)
        {
            return null;
        }

        private Task ComputeHashesForNewFiles(Node change, IProgress<BackupProgress> progress)
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
                // upload all files in a loop
                // update progress
                progress.Report(new BackupProgress());
            }

            return null;
        }

        private async Task SaveBackup(Node change)
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