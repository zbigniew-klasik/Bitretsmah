using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
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
        public async Task Test()
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

            var backupRepositoryMock = new Mock<IBackupRepository>();
            var hashServiceMock = new Mock<IHashService>();
            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();

            var changedFilesUploaderMock = new Mock<IChangedFilesUploader>();

            IBackupService backupService = new BackupService(
                backupRepositoryMock.Object,
                changedFilesUploaderMock.Object,
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

            localFilesServiceMock.Verify(x => x.GetNodeStructure(localPath));
            historyServiceMock.Verify(x => x.GetLastStructure(targetName));
            nodeChangesFinderMock.Verify(x => x.Find(previousStructure, currentStructure));

            // TODO
        }
    }
}