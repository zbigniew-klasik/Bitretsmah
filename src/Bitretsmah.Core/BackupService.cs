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

        Task Restore(RestoreRequest request);
    }

    public class BackupService : IBackupService
    {
        private readonly IBackupRepository _backupRepository;
        private readonly IChangedFilesDownloader _changedFilesDownloader;
        private readonly IChangedFilesUploader _changedFilesUploader;
        private readonly IDateTimeService _dateTimeService;
        private readonly IFileHashService _fileHashService;
        private readonly IHistoryService _historyService;
        private readonly ILocalFilesService _localFilesService;
        private readonly INodeChangesFinder _nodeChangesFinder;
        private readonly ITargetService _targetService;

        public BackupService(IBackupRepository backupRepository, IChangedFilesDownloader changedFilesDownloader, IChangedFilesUploader changedFilesUploader, IDateTimeService dateTimeService, IFileHashService fileHashService, IHistoryService historyService, ILocalFilesService localFilesService, INodeChangesFinder nodeChangesFinder, ITargetService targetService)
        {
            _backupRepository = backupRepository;
            _changedFilesDownloader = changedFilesDownloader;
            _changedFilesUploader = changedFilesUploader;
            _dateTimeService = dateTimeService;
            _fileHashService = fileHashService;
            _historyService = historyService;
            _localFilesService = localFilesService;
            _nodeChangesFinder = nodeChangesFinder;
            _targetService = targetService;
        }

        public async Task Backup(BackupRequest request)
        {
            var currentStructure = await GetCurrentStructure(request.TargetName, request.ComputeHashForEachFile, request.Progress);
            var previousStructure = await _historyService.GetLastStructure(request.TargetName);
            var structureChange = _nodeChangesFinder.Find(previousStructure, currentStructure);
            await _changedFilesUploader.Upload(structureChange, request.Progress);
            await SaveBackup(request, structureChange);
        }

        public async Task Restore(RestoreRequest request)
        {
            var currentStructure = await GetCurrentStructure(request.TargetName, request.ComputeHashForEachFile, request.Progress);
            var destinationStructure = await _historyService.GetLastStructure(request.TargetName);
            var structureChange = _nodeChangesFinder.Find(currentStructure, destinationStructure);
            await _changedFilesDownloader.Download(structureChange, request.Progress);
        }

        private async Task<Node> GetCurrentStructure(string targetName, bool computeHashForEachFile, IProgress<BackupProgress> progress)
        {
            var target = await _targetService.GetByName(targetName);
            var currentStructure = _localFilesService.GetNodeStructure(target.LocalPath);

            if (computeHashForEachFile)
            {
                await _fileHashService.TryEnsureEachFileHasComputedHash(currentStructure, progress);
            }

            return currentStructure;
        }

        private async Task SaveBackup(BackupRequest request, Node change)
        {
            var backup = new Backup
            {
                TargetName = request.TargetName,
                StructureChange = change,
                CreationTime = _dateTimeService.Now
            };

            await _backupRepository.Add(backup);

            // TODO:
            // use remote backup repository to upload backup to Mega
        }
    }
}