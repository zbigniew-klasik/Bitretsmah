using Bitretsmah.Core;
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
        public async Task CallAllServicesWithProperArguments()
        {
            var localPath = @"C:\Temp\Directory for backup";
            var currentStructure = new Directory();
            var localFilesServiceMock = new Mock<ILocalFilesService>();
            localFilesServiceMock.Setup(x => x.GetNodeStructure(localPath)).Returns(currentStructure);

            var targetName = @"Test Target name";
            var previousStructure = new Directory();
            var historyServiceMock = new Mock<IHistoryService>();
            historyServiceMock.Setup(x => x.GetLastStructure(targetName)).ReturnsAsync(previousStructure);

            var structureChange = new Directory();
            var nodeChangesFinderMock = new Mock<INodeChangesFinder>();
            nodeChangesFinderMock.Setup(x => x.Find(previousStructure, currentStructure)).Returns(structureChange);

            var now = DateTimeOffset.Now;
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.SetupGet(x => x.Now).Returns(now);

            var hashServiceMock = new Mock<IHashService>();
            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            var changedFilesUploaderMock = new Mock<IChangedFilesUploader>();

            Backup savedBackup = null;
            var backupRepositoryMock = new Mock<IBackupRepository>();
            backupRepositoryMock
                .Setup(x => x.Add(It.IsAny<Backup>()))
                .Returns(Task.CompletedTask)
                .Callback<Backup>(x => savedBackup = x);

            IBackupService backupService = new BackupService(
                backupRepositoryMock.Object,
                changedFilesUploaderMock.Object,
                dateTimeServiceMock.Object,
                hashServiceMock.Object,
                historyServiceMock.Object,
                localFilesServiceMock.Object,
                nodeChangesFinderMock.Object);

            var request = new BackupRequest
            {
                Target = targetName,
                LocalPath = localPath,
                ComputeHashForEachFile = false,
                Progress = new Progress<BackupProgress>()
            };

            await backupService.Backup(request);

            localFilesServiceMock.Verify(x => x.GetNodeStructure(localPath), Times.Once);
            historyServiceMock.Verify(x => x.GetLastStructure(targetName), Times.Once);
            nodeChangesFinderMock.Verify(x => x.Find(previousStructure, currentStructure), Times.Once);
            changedFilesUploaderMock.Verify(x => x.Upload(structureChange, request.Progress), Times.Once);
            dateTimeServiceMock.VerifyGet(x => x.Now, Times.Once);
            backupRepositoryMock.Verify(x => x.Add(It.IsAny<Backup>()), Times.Once);

            savedBackup.Should().NotBeNull();
            savedBackup.Target.Should().Be(targetName);
            savedBackup.StructureChange.Should().Be(structureChange);
            savedBackup.CreationTime.Should().Be(now);
        }
    }
}