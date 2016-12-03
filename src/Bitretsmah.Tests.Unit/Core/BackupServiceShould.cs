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

            var change = new Directory();
            var nodeChangesFinderMock = new Mock<INodeChangesFinder>();
            nodeChangesFinderMock.Setup(x => x.Find(previousStructure, currentStructure)).Returns(change);

            var backupRepositoryMock = new Mock<IBackupRepository>();
            var hashServiceMock = new Mock<IHashService>();
            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();

            IBackupService backupService = new BackupService(
                backupRepositoryMock.Object,
                hashServiceMock.Object,
                historyServiceMock.Object,
                localFilesServiceMock.Object,
                nodeChangesFinderMock.Object,
                remoteFileWarehouseFactoryMock.Object);

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