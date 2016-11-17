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

        public async Task CreateBackup(string path)
        {
            // get last time data
            var a = await _backupRepository.GetLastForPath(path);

            // get local structure
            var b = _localFilesService.GetNodeStructure(path);

            // compare local with last time
            var c = _nodeChangesFinder.Find(a.Node, b);

            // compute hashes
            _hashService.ComputeFileHash(b.AbsolutePath); // for each new file

            // upload files and save files info
            using (var warehouse = _remoteFileWarehouseFactory.Create())
            {
                // upload
                // save file info
            }

            // save backup info
            await _backupRepository.Add(new Backup());

            throw new NotImplementedException();
        }
    }
}