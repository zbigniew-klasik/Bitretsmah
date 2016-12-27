using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public interface IBackupService
    {
        Task Backup(BackupRequest request);
    }

    public class BackupService : IBackupService
    {
        private readonly IBackupRepository _backupRepository;
        private readonly IChangedFilesUploader _changedFilesUploader;
        private readonly IHashService _hashService;
        private readonly IHistoryService _historyService;
        private readonly ILocalFilesService _localFilesService;
        private readonly INodeChangesFinder _nodeChangesFinder;

        public BackupService(IBackupRepository backupRepository, IChangedFilesUploader changedFilesUploader, IHashService hashService, IHistoryService historyService, ILocalFilesService localFilesService, INodeChangesFinder nodeChangesFinder)
        {
            _backupRepository = backupRepository;
            _changedFilesUploader = changedFilesUploader;
            _hashService = hashService;
            _historyService = historyService;
            _localFilesService = localFilesService;
            _nodeChangesFinder = nodeChangesFinder;
        }

        public async Task Backup(BackupRequest request)
        {
            EnsureArg.IsNotNull(request, nameof(request));
            EnsureArg.IsNotNullOrWhiteSpace(request.Target, nameof(request.Target));
            EnsureArg.IsNotNullOrWhiteSpace(request.LocalPath, nameof(request.LocalPath));

            var currentStructure = _localFilesService.GetNodeStructure(request.LocalPath);

            if (request.ComputeHashForEachFile)
            {
                await ComputeHashesForAllFiles(currentStructure, request.Progress);
            }

            var previousStructure = await _historyService.GetLastStructure(request.Target);

            var structureChange = _nodeChangesFinder.Find(previousStructure, currentStructure);

            await _changedFilesUploader.Upload(structureChange, request.Progress);

            await SaveBackup(structureChange, request.Progress);
        }

        private Task ComputeHashesForAllFiles(Node change, IProgress<BackupProgress> progress)
        {
            // todo: foreach
            _hashService.ComputeFileHash(change.AbsolutePath);

            // update progress
            // progress.Report(new BackupProgress());

            return Task.Run(() => _hashService.ComputeFileHash(change.AbsolutePath));
        }

        private async Task SaveBackup(Node change, IProgress<BackupProgress> progress)
        {
            // TODO:
            // create new backup entity
            // upload backup to Mega
            // save backup in local DB
            // calculate indexes and save to local DB

            // local backup repo
            // remote backup repo
            await _backupRepository.Add(new Backup());
        }
    }
}