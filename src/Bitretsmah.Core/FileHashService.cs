﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Bitretsmah.Core.Exceptions;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using EnsureThat;

namespace Bitretsmah.Core
{
    public interface IFileHashService
    {
        Task TryEnsureEachFileHasComputedHash(Node node, IProgress<BackupProgress> progress);

        Task TryEnsureFileHasComputedHash(File file, IProgress<BackupProgress> progress);

        Task VerifyFileHash(File file, IProgress<BackupProgress> progress);
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
            var filesList = node.StructureToList().OfType<File>().ToList();
            var processedFilesNumber = 0;

            foreach (var file in filesList)
            {
                await TryEnsureFileHasComputedHash(file, progress, filesList.Count, processedFilesNumber);
                processedFilesNumber++;
            }
        }

        public async Task TryEnsureFileHasComputedHash(File file, IProgress<BackupProgress> progress)
        {
            await TryEnsureFileHasComputedHash(file, progress, 1, 0);
        }

        public Task VerifyFileHash(File file, IProgress<BackupProgress> progress)
        {
            EnsureArg.IsNotNull(file);
            EnsureArg.IsNotNullOrWhiteSpace(file.Hash);

            return Task.Run(() =>
            {
                var actualFileHash = _fileHashProvider.ComputeFileHash(file.AbsolutePath);
                if (!actualFileHash.Equals(file.Hash))
                    throw new InvalidFileHashException(file.AbsolutePath, file.Hash, actualFileHash);
            });
        }

        private Task TryEnsureFileHasComputedHash(File file, IProgress<BackupProgress> progress, int allFilesNumber,
            int processedFilesNumber)
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