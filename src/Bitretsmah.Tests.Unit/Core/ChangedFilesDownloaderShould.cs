using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Moq;
using NUnit.Framework;
using static Bitretsmah.Tests.Unit.Core.NodesTestHelper;

namespace Bitretsmah.Tests.Unit.Core
{
    public class ChangedFilesDownloaderShould
    {
        private IChangedFilesDownloader _changedFilesDownloader;
        private Mock<IFileHashService> _fileHashServiceMock;
        private Mock<ILocalFilesService> _localFilesServiceMock;
        private Mock<ILogger> _loggerMock;
        private Mock<IRemoteFileWarehouseFactory> _remoteFileWarehouseFactoryMock;
        private Mock<IRemoteFileWarehouse> _remoteFileWarehouseMock;

        [SetUp]
        public void Setup()
        {
            _fileHashServiceMock = new Mock<IFileHashService>();
            _localFilesServiceMock = new Mock<ILocalFilesService>();
            _loggerMock = new Mock<ILogger>();

            _remoteFileWarehouseMock = new Mock<IRemoteFileWarehouse>();
            _remoteFileWarehouseMock.Setup(x => x.GetFilesList()).ReturnsAsync(new List<RemoteFile>());

            _remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            _remoteFileWarehouseFactoryMock.Setup(x => x.Create()).ReturnsAsync(_remoteFileWarehouseMock.Object);

            _changedFilesDownloader = new ChangedFilesDownloader(_fileHashServiceMock.Object,
                _localFilesServiceMock.Object, _loggerMock.Object, _remoteFileWarehouseFactoryMock.Object);
        }

        [Test]
        public async Task CreateWarehouse()
        {
            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                CreateFile("file", NodeState.Created));

            await _changedFilesDownloader.Download(filesStructureChange, new Progress<BackupProgress>());

            _remoteFileWarehouseFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public async Task NotCreateWarehouseWhenNothingToDownload()
        {
            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                CreateFile("file", NodeState.Deleted));

            await _changedFilesDownloader.Download(filesStructureChange, new Progress<BackupProgress>());

            _remoteFileWarehouseFactoryMock.Verify(x => x.Create(), Times.Never);
        }

        [Test]
        public async Task DeleteAllFilesAndDirectories()
        {
            var dir2 = CreateDirectory("dir2", NodeState.Deleted);
            var file2 = CreateFile("file2", NodeState.Deleted);

            var dir1 = CreateDirectory("dir1", NodeState.Deleted, file2, dir2);
            var file1 = CreateFile("file1", NodeState.Deleted);

            var filesStructureChange = CreateDirectory("root", NodeState.Modified, dir1, file1);

            await _changedFilesDownloader.Download(filesStructureChange, new Progress<BackupProgress>());

            _localFilesServiceMock.Verify(x => x.DeleteFileOrDirectory(dir2.AbsolutePath));
            _localFilesServiceMock.Verify(x => x.DeleteFileOrDirectory(file2.AbsolutePath));
            _localFilesServiceMock.Verify(x => x.DeleteFileOrDirectory(dir1.AbsolutePath));
            _localFilesServiceMock.Verify(x => x.DeleteFileOrDirectory(file1.AbsolutePath));
        }

        [Test]
        public async Task DownloadAllCorrectFiles()
        {
            var fileA = CreateFile("fileA", NodeState.Deleted);
            var fileB = CreateFile("fileB", NodeState.Modified);

            var dir1 = CreateDirectory("dir1", NodeState.Modified, fileA, fileB);
            var fileC = CreateFile("fileC", NodeState.Created);

            var filesStructureChange = CreateDirectory("root", NodeState.Modified, dir1, fileC);

            var progress = new Progress<BackupProgress>();

            await _changedFilesDownloader.Download(filesStructureChange, progress);

            _remoteFileWarehouseMock.Verify(x => x.DownloadFile(fileA.RemoteId, It.IsAny<Progress<double>>()), Times.Never);
            _localFilesServiceMock.Verify(x => x.WriteFileStream(fileA.AbsolutePath, It.IsAny<Stream>()), Times.Never);
            _fileHashServiceMock.Verify(x => x.VerifyFileHash(fileA, progress), Times.Never);

            _remoteFileWarehouseMock.Verify(x => x.DownloadFile(fileB.RemoteId, It.IsAny<Progress<double>>()), Times.Once);
            _localFilesServiceMock.Verify(x => x.WriteFileStream(fileB.AbsolutePath, It.IsAny<Stream>()), Times.Once);
            _fileHashServiceMock.Verify(x => x.VerifyFileHash(fileB, progress), Times.Once);

            _remoteFileWarehouseMock.Verify(x => x.DownloadFile(fileC.RemoteId, It.IsAny<Progress<double>>()), Times.Once);
            _localFilesServiceMock.Verify(x => x.WriteFileStream(fileC.AbsolutePath, It.IsAny<Stream>()), Times.Once);
            _fileHashServiceMock.Verify(x => x.VerifyFileHash(fileC, progress), Times.Once);
        }

        [Test]
        public async Task SetProperCreationAndModificationTimes()
        {
            var fileA = CreateFile("fileA", NodeState.Created);
            fileA.CreationTime = new DateTimeOffset(2017, 4, 11, 10, 12, 13, new TimeSpan());
            fileA.ModificationTime = new DateTimeOffset(2017, 5, 15, 18, 14, 11, new TimeSpan());

            var filesStructureChange = CreateDirectory("root", NodeState.Modified, fileA);
            var progress = new Progress<BackupProgress>();
            await _changedFilesDownloader.Download(filesStructureChange, progress);

            _localFilesServiceMock.Verify(x => x.SetCreationTime(fileA.AbsolutePath, fileA.CreationTime), Times.Once);
            _localFilesServiceMock.Verify(x => x.SetLastWriteTime(fileA.AbsolutePath, fileA.ModificationTime), Times.Once);
        }
    }
}