using Bitretsmah.Core.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Bitretsmah.Core.Models;

namespace Bitretsmah.Data.System
{
    public class HashService : IHashService
    {
        private readonly ILogger _logger;

        public string ComputeFileHash(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            using (var stream = fileInfo.Open(FileMode.Open))
            {
                stream.Position = 0;
                var sha1 = new SHA1Managed();
                var hash = sha1.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public async Task TryEnsureEachFileHasComputedHash(Node node, IProgress<BackupProgress> progress)
        {
            // TODO: unit tests
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
            // TODO: unit tests
            await TryEnsureFileHasComputedHash(file, progress, 1, 0);
        }

        private Task TryEnsureFileHasComputedHash(Core.Models.File file, IProgress<BackupProgress> progress, int allFilesNumber, int processedFilesNumber)
        {
            return Task.Run(() =>
            {
                try
                {
                    progress.Report(BackupProgress.CreateHashStartReport(allFilesNumber, processedFilesNumber, file));
                    file.Hash = ComputeFileHash(file.AbsolutePath);
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