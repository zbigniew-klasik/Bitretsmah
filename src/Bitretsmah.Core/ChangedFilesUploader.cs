﻿using Bitretsmah.Core.Interfaces;
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
        private readonly IRemoteFileWarehouseFactory _remoteFileWarehouseFactory;

        public ChangedFilesUploader(IHashService hashService, ILocalFilesService localFilesService, IRemoteFileWarehouseFactory remoteFileWarehouseFactory)
        {
            _hashService = hashService;
            _localFilesService = localFilesService;
            _remoteFileWarehouseFactory = remoteFileWarehouseFactory;
        }

        public async Task Upload(Node filesStructureChange, IProgress<BackupProgress> progress)
        {
            EnsureArg.IsNotNull(filesStructureChange);
            EnsureArg.IsNotNull(progress);

            var createdAndModifiedFiles =
                filesStructureChange.StructureToList()
                    .Where(x => x.State == NodeState.Created || x.State == NodeState.Modified)
                    .Where(x => x is File)
                    .Select(x => (File)x)
                    .ToList();

            if (!createdAndModifiedFiles.Any()) return;

            using (var warehouse = _remoteFileWarehouseFactory.Create())
            {
                var uploadedFilesHashes = await GetUploadedFilesHashes(warehouse);

                foreach (var file in createdAndModifiedFiles)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(file.Hash))
                        {
                            _hashService.ComputeFileHash(file.AbsolutePath);
                        }

                        if (uploadedFilesHashes.All(x => x != file.Hash))
                        {
                            var stream = _localFilesService.ReadFileStream(file.AbsolutePath);
                            await warehouse.UploadFile(stream, $"[{file.Hash}]_{file.Name}", new Progress<double>());
                        }

                        // progress.Report(new BackupProgress());
                    }
                    catch (Exception)
                    {
                        // TODO
                        throw;
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