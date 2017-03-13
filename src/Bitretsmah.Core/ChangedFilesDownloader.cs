using System;
using System.Linq;
using System.Threading.Tasks;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using EnsureThat;

namespace Bitretsmah.Core
{
    public interface IChangedFilesDownloader
    {
        Task Download(Node filesStructureChange, IProgress<BackupProgress> progress);
    }

    public class ChangedFilesDownloader : IChangedFilesDownloader
    {
        private readonly IFileHashService _fileHashService;
        private readonly ILocalFilesService _localFilesService;
        private readonly ILogger _logger;
        private readonly IRemoteFileWarehouseFactory _remoteFileWarehouseFactory;

        public ChangedFilesDownloader(IFileHashService fileHashService, ILocalFilesService localFilesService,
            ILogger logger, IRemoteFileWarehouseFactory remoteFileWarehouseFactory)
        {
            _fileHashService = fileHashService;
            _localFilesService = localFilesService;
            _logger = logger;
            _remoteFileWarehouseFactory = remoteFileWarehouseFactory;
        }

        public async Task Download(Node filesStructureChange, IProgress<BackupProgress> progress)
        {
            // TODO: unit test all of it!!!!!!

            EnsureArg.IsNotNull(filesStructureChange);
            EnsureArg.IsNotNull(progress);

            var createdAndModifiedFiles =
                filesStructureChange.StructureToList()
                    .Where(x => x.State == NodeState.Created || x.State == NodeState.Modified).OfType<File>()
                    .ToList();

            if (!createdAndModifiedFiles.Any()) return;

            using (var warehouse = await _remoteFileWarehouseFactory.Create())
            {
                int processedFilesNumber = 0;

                foreach (var file in createdAndModifiedFiles)
                {
                    try
                    {
                        progress.Report(BackupProgress.CreateDownloadStartReport(createdAndModifiedFiles.Count, processedFilesNumber, file));

                        var downloadProgress = new Progress<double>(downloadPercentage =>
                                    progress.Report(BackupProgress.CreateUploadProgressReport(createdAndModifiedFiles.Count, processedFilesNumber, file, downloadPercentage)));

                        using (var stream = await warehouse.DownloadFile(file.RemoteId, downloadProgress))
                        {
                            _localFilesService.WriteFileStream("path", stream);
                        }

                        await _fileHashService.VerifyFileHash(file, progress);

                        progress.Report(BackupProgress.CreateDownloadFinishedReport(createdAndModifiedFiles.Count, processedFilesNumber, file));

                        processedFilesNumber++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Could not process file: '{0}'.", file.AbsolutePath);
                        progress.Report(BackupProgress.CreateErrorReport($"Could not process file: '{file.Name}'."));
                    }
                }
            }
        }
    }
}