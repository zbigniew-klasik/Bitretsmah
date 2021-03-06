﻿using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class BackupServiceShould
    {
        [Test]
        public async Task Backup_CallAllServicesWithProperArguments()
        {
            var targetName = @"Test Target name";
            var localPath = @"C:\Temp\Directory for backup";
            var target = new Target { Name = targetName, LocalPath = localPath };
            var targetServiceMock = new Mock<ITargetService>();
            targetServiceMock.Setup(x => x.GetByName(targetName)).ReturnsAsync(target);

            var currentStructure = new Directory();
            var localFilesServiceMock = new Mock<ILocalFilesService>();
            localFilesServiceMock.Setup(x => x.GetNodeStructure(localPath)).Returns(currentStructure);

            var previousStructure = new Directory();
            var historyServiceMock = new Mock<IHistoryService>();
            historyServiceMock.Setup(x => x.GetLastStructure(targetName)).ReturnsAsync(previousStructure);

            var structureChange = new Directory();
            var nodeChangesFinderMock = new Mock<INodeChangesFinder>();
            nodeChangesFinderMock.Setup(x => x.Find(previousStructure, currentStructure)).Returns(structureChange);

            var now = DateTimeOffset.Now;
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.SetupGet(x => x.Now).Returns(now);

            var fileHashServiceMock = new Mock<IFileHashService>();
            var changedFilesDownloaderMock = new Mock<IChangedFilesDownloader>();
            var changedFilesUploaderMock = new Mock<IChangedFilesUploader>();

            Backup savedBackup = null;
            var backupRepositoryMock = new Mock<IBackupRepository>();
            backupRepositoryMock
                .Setup(x => x.Add(It.IsAny<Backup>()))
                .Returns(Task.CompletedTask)
                .Callback<Backup>(x => savedBackup = x);

            IBackupService backupService = new BackupService(
                backupRepositoryMock.Object,
                changedFilesDownloaderMock.Object,
                changedFilesUploaderMock.Object,
                dateTimeServiceMock.Object,
                fileHashServiceMock.Object,
                historyServiceMock.Object,
                localFilesServiceMock.Object,
                nodeChangesFinderMock.Object,
                targetServiceMock.Object);

            var request = new BackupRequest
            {
                TargetName = targetName,
                ComputeHashForEachFile = true,
                Progress = new Progress<BackupProgress>()
            };

            await backupService.Backup(request);

            localFilesServiceMock.Verify(x => x.GetNodeStructure(localPath), Times.Once);
            fileHashServiceMock.Verify(x => x.TryEnsureEachFileHasComputedHash(currentStructure, request.Progress), Times.Once);
            historyServiceMock.Verify(x => x.GetLastStructure(targetName), Times.Once);
            nodeChangesFinderMock.Verify(x => x.Find(previousStructure, currentStructure), Times.Once);
            changedFilesUploaderMock.Verify(x => x.Upload(structureChange, request.Progress), Times.Once);
            dateTimeServiceMock.VerifyGet(x => x.Now, Times.Once);
            backupRepositoryMock.Verify(x => x.Add(It.IsAny<Backup>()), Times.Once);

            savedBackup.Should().NotBeNull();
            savedBackup.TargetName.Should().Be(targetName);
            savedBackup.StructureChange.Should().Be(structureChange);
            savedBackup.CreationTime.Should().Be(now);
        }

        [Test]
        public async Task Restore_CallAllServicesWithProperArguments()
        {
            var targetName = @"Test Target name";
            var localPath = @"C:\Temp\Directory to restore";
            var target = new Target { Name = targetName, LocalPath = localPath };
            var targetServiceMock = new Mock<ITargetService>();
            targetServiceMock.Setup(x => x.GetByName(targetName)).ReturnsAsync(target);

            var currentStructure = new Directory();
            var localFilesServiceMock = new Mock<ILocalFilesService>();
            localFilesServiceMock.Setup(x => x.GetNodeStructure(localPath)).Returns(currentStructure);

            var destinationStructure = new Directory();
            var historyServiceMock = new Mock<IHistoryService>();
            historyServiceMock.Setup(x => x.GetLastStructure(targetName)).ReturnsAsync(destinationStructure);

            var structureChange = new Directory();
            var nodeChangesFinderMock = new Mock<INodeChangesFinder>();
            nodeChangesFinderMock.Setup(x => x.Find(currentStructure, destinationStructure)).Returns(structureChange);

            var dateTimeServiceMock = new Mock<IDateTimeService>();
            var fileHashServiceMock = new Mock<IFileHashService>();
            var changedFilesDownloaderMock = new Mock<IChangedFilesDownloader>();
            var changedFilesUploaderMock = new Mock<IChangedFilesUploader>();
            var backupRepositoryMock = new Mock<IBackupRepository>();

            IBackupService backupService = new BackupService(
                backupRepositoryMock.Object,
                changedFilesDownloaderMock.Object,
                changedFilesUploaderMock.Object,
                dateTimeServiceMock.Object,
                fileHashServiceMock.Object,
                historyServiceMock.Object,
                localFilesServiceMock.Object,
                nodeChangesFinderMock.Object,
                targetServiceMock.Object);

            var request = new RestoreRequest
            {
                TargetName = targetName,
                ComputeHashForEachFile = true,
                Progress = new Progress<BackupProgress>()
            };

            await backupService.Restore(request);

            localFilesServiceMock.Verify(x => x.GetNodeStructure(localPath), Times.Once);
            fileHashServiceMock.Verify(x => x.TryEnsureEachFileHasComputedHash(currentStructure, request.Progress), Times.Once);
            historyServiceMock.Verify(x => x.GetLastStructure(targetName), Times.Once);
            nodeChangesFinderMock.Verify(x => x.Find(currentStructure, destinationStructure), Times.Once);
            changedFilesDownloaderMock.Verify(x => x.Download(structureChange, request.Progress), Times.Once);
        }
    }
}