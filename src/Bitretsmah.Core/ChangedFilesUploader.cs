using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public interface IChangedFilesUploader
    {
        Task Upload(Node filesStructureChange, IProgress<BackupProgress> progress);
    }

    public class ChangedFilesUploader : IChangedFilesUploader
    {
        private readonly IHashService _hashService;
        private readonly IRemoteFileWarehouseFactory _remoteFileWarehouseFactory;

        public ChangedFilesUploader(IHashService hashService, IRemoteFileWarehouseFactory remoteFileWarehouseFactory)
        {
            _hashService = hashService;
            _remoteFileWarehouseFactory = remoteFileWarehouseFactory;
        }

        public async Task Upload(Node filesStructureChange, IProgress<BackupProgress> progress)
        {
            EnsureArg.IsNotNull(filesStructureChange);
            EnsureArg.IsNotNull(progress);

            var createdAndModifiedFiles =
                filesStructureChange.StructureToList()
                    .Where(x => x is File)
                    .Where(x => x.State == NodeState.Created || x.State == NodeState.Modified)
                    .ToList();

            if (!createdAndModifiedFiles.Any()) return;

            using (var warehouse = _remoteFileWarehouseFactory.Create())
            {
                foreach (var file in createdAndModifiedFiles)
                {
                    try
                    {
                        // compute hash if not computed yet
                        _hashService.ComputeFileHash(filesStructureChange.AbsolutePath);

                        // upload file if it does not exist in warehouse yet

                        // progress.Report(new BackupProgress());
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}