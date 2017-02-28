using System;
using System.Linq;
using System.Threading.Tasks;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;

namespace Bitretsmah.Core
{
    public interface IFileHashService
    {
        Task TryEnsureEachFileHasComputedHash(Node node, IProgress<BackupProgress> progress);

        Task TryEnsureFileHasComputedHash(Core.Models.File file, IProgress<BackupProgress> progress);
    }

    public class FileHashService : IFileHashService
    {
        private readonly IFileHashProvider _fileHashProvider;
        private readonly ILogger _logger;

        public FileHashService(IFileHashProvider fileHashProvider, ILogger logger)
        {
            _fileHashProvider = fileHashProvider;
            _logger = logger;
        }

        public async Task TryEnsureEachFileHasComputedHash(Node node, IProgress<BackupProgress> progress)
        {
            var filesList = node.StructureToList().OfType<Core.Models.File>().ToList();
            var processedFilesNumber = 0;

            foreach (var file in filesList)
            {
                await TryEnsureFileHasComputedHash(file, progress, filesList.Count, processedFilesNumber);
                processedFilesNumber++;
            }
        }

        public async Task TryEnsureFileHasComputedHash(Core.Models.File file, IProgress<BackupProgress> progress)
        {
            await TryEnsureFileHasComputedHash(file, progress, 1, 0);
        }

        private Task TryEnsureFileHasComputedHash(Core.Models.File file, IProgress<BackupProgress> progress, int allFilesNumber, int processedFilesNumber)
        {
            return Task.Run(() =>
            {
                try
                {
                    progress.Report(BackupProgress.CreateHashStartReport(allFilesNumber, processedFilesNumber, file));
                    file.Hash = _fileHashProvider.ComputeFileHash(file.AbsolutePath);
                    progress.Report(BackupProgress.CreateHashFinishedReport(allFilesNumber, processedFilesNumber, file));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Could not process file: '{0}'.", file.AbsolutePath);
                    progress.Report(BackupProgress.CreateErrorReport($"Could not process file: '{file.Name}'."));
                }
            });
        }
    }
}