using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly ILocalFilesService _localFilesService;
        private readonly ILogger _logger;
        private readonly IRemoteFileWarehouseFactory _remoteFileWarehouseFactory;

        public ChangedFilesUploader(IHashService hashService, ILocalFilesService localFilesService, ILogger logger, IRemoteFileWarehouseFactory remoteFileWarehouseFactory)
        {
            _hashService = hashService;
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
                var uploadedFilesHashes = await GetUploadedFilesHashes(warehouse);

                int processedFilesNumber = 0;

                foreach (var file in createdAndModifiedFiles)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(file.Hash))
                        {
                            // TODO: unit tests
                            await _hashService.TryEnsureFileHasComputedHash(file, progress);
                        }

                        if (uploadedFilesHashes.All(x => x != file.Hash))
                        {
                            progress.Report(BackupProgress.CreateUploadStartReport(createdAndModifiedFiles.Count, processedFilesNumber, file));

                            using (var stream = _localFilesService.ReadFileStream(file.AbsolutePath))
                            {
                                var uploadProgress = new Progress<double>(uploadPercentage =>
                                    progress.Report(BackupProgress.CreateUploadProgressReport(createdAndModifiedFiles.Count, processedFilesNumber, file, uploadPercentage)));

                                file.RemoteId = await warehouse.UploadFile(stream, $"[{file.Hash}]_{file.Name}", uploadProgress);
                            }

                            progress.Report(BackupProgress.CreateUploadFinishedReport(createdAndModifiedFiles.Count, processedFilesNumber, file));
                        }

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

        private static async Task<List<string>> GetUploadedFilesHashes(IRemoteFileWarehouse warehouse)
        {
            var query =
                from file in await warehouse.GetFilesList()
                let match = Regex.Match(file.Name, @"(?<=^\[)[0-9A-F]{40}(?=\]_.*$)", RegexOptions.Singleline | RegexOptions.CultureInvariant)
                where match.Success
                select match.Value;

            return query.ToList();
        }
    }
}