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

        public Task TryEnsureEachFileHasComputedHash(Node node, IProgress<BackupProgress> progress)
        {
            // TODO: unit tests
            return Task.Run(() =>
            {
                var filesList = node.StructureToList().OfType<Core.Models.File>().ToList();
                var processedFilesNumber = 0;

                foreach (var file in filesList)
                {
                    try
                    {
                        progress.Report(BackupProgress.CreateHashStartReport(filesList.Count, processedFilesNumber, file));
                        file.Hash = ComputeFileHash(file.AbsolutePath);
                        progress.Report(BackupProgress.CreateHashFinishedReport(filesList.Count, processedFilesNumber, file));
                        processedFilesNumber++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Could not process file: '{0}'.", file.AbsolutePath);
                        progress.Report(BackupProgress.CreateErrorReport($"Could not process file: '{file.Name}'."));
                    }

                }
            });
        }

        public Task TryEnsureFileHasComputedHash(Core.Models.File file, IProgress<BackupProgress> progress)
        {
            // TODO: unit tests
            return TryEnsureFileHasComputedHash(file, progress, 0, 0);
            throw new NotImplementedException();
        }

        private Task TryEnsureFileHasComputedHash(Core.Models.File file, IProgress<BackupProgress> progress, int processedFilesNumber, int allFilesNumber)
        {
            throw new NotImplementedException();
        }
    }
}