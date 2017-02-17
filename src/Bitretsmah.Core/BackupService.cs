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
        private readonly IDateTimeService _dateTimeService;
        private readonly IHashService _hashService;
        private readonly IHistoryService _historyService;
        private readonly ILocalFilesService _localFilesService;
        private readonly INodeChangesFinder _nodeChangesFinder;
        private readonly ITargetService _targetService;

        public BackupService(IBackupRepository backupRepository, IChangedFilesUploader changedFilesUploader, IDateTimeService dateTimeService, IHashService hashService, IHistoryService historyService, ILocalFilesService localFilesService, INodeChangesFinder nodeChangesFinder, ITargetService targetService)
        {
            _backupRepository = backupRepository;
            _changedFilesUploader = changedFilesUploader;
            _dateTimeService = dateTimeService;
            _hashService = hashService;
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
            //targetStructure
            //structureChange
            //downloadChanges
            throw new NotImplementedException();
        }

        private async Task<Node> GetCurrentStructure(string targetName, bool computeHashForEachFile, IProgress<BackupProgress> progress)
        {
            var target = await _targetService.GetByName(targetName);
            var currentStructure = _localFilesService.GetNodeStructure(target.LocalPath);

            if (computeHashForEachFile)
            {
                await ComputeHashesForAllFiles(currentStructure, progress);
            }

            return currentStructure;
        }

        private Task ComputeHashesForAllFiles(Node change, IProgress<BackupProgress> progress)
        {
            // TODO: use HashService

            // todo: foreach
            _hashService.ComputeFileHash(change.AbsolutePath);

            // update progress
            // progress.Report(new BackupProgress());

            return Task.Run(() => _hashService.ComputeFileHash(change.AbsolutePath));
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