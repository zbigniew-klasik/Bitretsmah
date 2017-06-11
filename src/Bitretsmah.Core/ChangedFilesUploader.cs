using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bitretsmah.Core.Exceptions;

namespace Bitretsmah.Core
{
    public interface IChangedFilesUploader
    {
        Task Upload(Node filesStructureChange, IProgress<BackupProgress> progress);
    }

    public class ChangedFilesUploader : IChangedFilesUploader
    {
        private readonly IFileHashService _fileHashService;
        private readonly ILocalFilesService _localFilesService;
        private readonly ILogger _logger;
        private readonly IRemoteFileWarehouseFactory _remoteFileWarehouseFactory;

        public ChangedFilesUploader(IFileHashService fileHashService, ILocalFilesService localFilesService, ILogger logger, IRemoteFileWarehouseFactory remoteFileWarehouseFactory)
        {
            _fileHashService = fileHashService;
            _localFilesService = localFilesService;
            _logger = logger;
            _remoteFileWarehouseFactory = remoteFileWarehouseFactory;
        }

        public async Task Upload(Node filesStructureChange, IProgress<BackupProgress> progress)
        {
            EnsureArg.IsNotNull(filesStructureChange);
            EnsureArg.IsNotNull(progress);

            var createdAndModifiedFiles =
                filesStructureChange.StructureToList()
                    .Where(x => x.State == NodeState.Created || x.State == NodeState.Modified).OfType<File>()
                    .ToList();

            if (!createdAndModifiedFiles.Any()) return;

            using (var warehouse = await _remoteFileWarehouseFactory.Create())
            {
                var remoteFiles = await warehouse.GetFilesList();

                int processedFilesNumber = 0;

                foreach (var file in createdAndModifiedFiles)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(file.Hash))
                        {
                            await _fileHashService.TryEnsureFileHasComputedHash(file, progress);
                        }

                        var matchingRemoteFile = remoteFiles.FirstOrDefault(x => x.Hash == file.Hash);

                        if (matchingRemoteFile == null)
                        {
                            progress.Report(BackupProgress.CreateUploadStartReport(createdAndModifiedFiles.Count, processedFilesNumber, file));

                            using (var stream = _localFilesService.ReadFileStream(file.AbsolutePath))
                            {
                                var uploadProgress = new Progress<double>(uploadPercentage =>
                                    progress.Report(BackupProgress.CreateUploadProgressReport(createdAndModifiedFiles.Count, processedFilesNumber, file, uploadPercentage)));

                                file.RemoteId = await warehouse.UploadFile(stream, $"[{file.Hash}]_{file.Name}", uploadProgress);
                            }

                            _logger.Info($"Uploaded file: '{file.AbsolutePath}' with remote id: '{file.RemoteId}'.");
                            progress.Report(BackupProgress.CreateUploadFinishedReport(createdAndModifiedFiles.Count, processedFilesNumber, file));
                        }
                        else
                        {
                            file.RemoteId = matchingRemoteFile.Id;
                            _logger.Info($"File: '{file.AbsolutePath}' is already on the server with remote id: '{file.RemoteId}'.");
                        }

                        processedFilesNumber++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Could not process file: '{0}'.", file.AbsolutePath);
                        progress.Report(BackupProgress.CreateErrorReport($"Could not process file: '{file.Name}'."));
                        throw new UploadFailedException($"Could not process file: '{file.Name}'.");
                    }
                }
            }
        }
    }
}