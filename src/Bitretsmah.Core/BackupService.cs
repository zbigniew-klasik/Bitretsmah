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

        public async Task CreateBackup(string path, IProgress<BackupProgress> progress)
        {
            var change = await GetChange(path);

            await ComputeHashes(change, progress);

            await UploadFiles(change, progress);

            // TODO: Upload Change

            await SaveBackup(change);

            throw new NotImplementedException();
        }

        private async Task<Node> GetChange(string path)
        {
            // get local structure
            var currentStructure = _localFilesService.GetNodeStructure(path);

            // get last time data
            var lastBackup = await _backupRepository.GetLastForPath(path);

            Node change;

            if (lastBackup == null)
            {
                // to do mark all as added
                change = null;
            }
            else
            {
                var previousStructure = lastBackup.Node;
                change = _nodeChangesFinder.Find(previousStructure, currentStructure);
            }

            return change;
        }

        private Task ComputeHashes(Node change, IProgress<BackupProgress> progress)
        {
            // todo: foreach
            _hashService.ComputeFileHash(change.AbsolutePath);

            progress.Report(new BackupProgress());

            return Task.Run(() => _hashService.ComputeFileHash(change.AbsolutePath));
        }

        private Task UploadFiles(Node change, IProgress<BackupProgress> progress)
        {
            // upload files and save files info
            using (var warehouse = _remoteFileWarehouseFactory.Create())
            {
                // upload
                // save file info
            }

            return null;
        }

        private async Task SaveBackup(Node change)
        {
            // todo save change
            // todo create and save backup
            await _backupRepository.Add(new Backup());
        }
    }
}