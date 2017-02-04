using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Bitretsmah.Tests.Unit.Core.NodesTestHelper;

namespace Bitretsmah.Tests.Unit.Core
{
    public class ChangedFilesUploaderShould
    {
        private Mock<IHashService> _hashServiceMock;
        private Mock<ILocalFilesService> _localFilesServiceMock;
        private Mock<IRemoteFileWarehouse> _remoteFileWarehouseMock;
        private Mock<IRemoteFileWarehouseFactory> _remoteFileWarehouseFactoryMock;
        private IChangedFilesUploader _changedFilesUploader;

        [SetUp]
        public void Setup()
        {
            _hashServiceMock = new Mock<IHashService>();

            _localFilesServiceMock = new Mock<ILocalFilesService>();

            _remoteFileWarehouseMock = new Mock<IRemoteFileWarehouse>();
            _remoteFileWarehouseMock.Setup(x => x.GetFilesList()).ReturnsAsync(new List<RemoteFile>());

            _remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            _remoteFileWarehouseFactoryMock.Setup(x => x.Create()).ReturnsAsync(_remoteFileWarehouseMock.Object);

            _changedFilesUploader = new ChangedFilesUploader(_hashServiceMock.Object, _localFilesServiceMock.Object, _remoteFileWarehouseFactoryMock.Object);
        }

        [Test]
        public async Task CreateWarehouse()
        {
            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                                            CreateFile("file", NodeState.Created));

            await _changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            _remoteFileWarehouseFactoryMock.Verify(x => x.Create(), Times.Once);
            _remoteFileWarehouseMock.Verify(x => x.GetFilesList(), Times.Once);
        }

        [Test]
        public async Task NotCreateWarehouseWhenNothingToUpload()
        {
            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                                            CreateFile("file", NodeState.Deleted));

            await _changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            _remoteFileWarehouseFactoryMock.Verify(x => x.Create(), Times.Never);
            _remoteFileWarehouseMock.Verify(x => x.GetFilesList(), Times.Never);
        }

        [Test]
        public async Task ComputeHashForEachFileWithEmptyHash()
        {
            var filesStructureChange = CreateDirectory("directory1", NodeState.Modified,
                                            CreateFile("file1.txt", NodeState.Created, "any hash"),
                                            CreateFile("file2.txt", NodeState.Modified, string.Empty),
                                            CreateFile("file3.txt", NodeState.Deleted, null),
                                            CreateDirectory("directory2", NodeState.Modified,
                                                CreateFile("file4.txt", NodeState.Created, null),
                                                CreateFile("file5.txt", NodeState.Modified, "any hash"),
                                                CreateFile("file6.txt", NodeState.Deleted, string.Empty)));

            await _changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            _hashServiceMock.Verify(x => x.ComputeFileHash(It.IsAny<string>()), Times.Exactly(2));
            _hashServiceMock.Verify(x => x.ComputeFileHash(@"C:\Temp\file2.txt"), Times.Once);
            _hashServiceMock.Verify(x => x.ComputeFileHash(@"C:\Temp\file4.txt"), Times.Once);
        }

        [Test]
        public async Task UploadsEachFileNotUploadedYet()
        {
            var fileStream = new MemoryStream();
            _localFilesServiceMock.Setup(x => x.ReadFileStream(@"C:\Temp\file_not_yet_uploaded.txt")).Returns(fileStream);

            var previouslyUploadedFiles = new List<RemoteFile>()
            {
                new RemoteFile { Name = "any_file.bin" },
                new RemoteFile { Name = "[A8499F0A32485CB682D5AB7050F967CFBA504D84]_file_uploaded_before.txt" }
            };

            var remoteId = new RemoteId("store id", "node id");

            _remoteFileWarehouseMock.Setup(x => x.GetFilesList()).ReturnsAsync(previouslyUploadedFiles);
            _remoteFileWarehouseMock.Setup(
                x =>
                    x.UploadFile(fileStream, "[F7031B995F0F2E9CED6D62156F3DEB756ADB7E1E]_file_not_yet_uploaded.txt",
                        It.IsAny<Progress<double>>())).ReturnsAsync(remoteId);

            var fileUploadedBefore = CreateFile("file_uploaded_before.txt", NodeState.Modified, "A8499F0A32485CB682D5AB7050F967CFBA504D84");
            var fileNotYetUploaded = CreateFile("file_not_yet_uploaded.txt", NodeState.Modified, "F7031B995F0F2E9CED6D62156F3DEB756ADB7E1E");
            var filesStructureChange = CreateDirectory("root", NodeState.Modified, fileUploadedBefore, fileNotYetUploaded);

            await _changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            _remoteFileWarehouseMock.Verify(x => x.UploadFile(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<Progress<double>>()), Times.Once);
            _remoteFileWarehouseMock.Verify(x => x.UploadFile(fileStream, "[F7031B995F0F2E9CED6D62156F3DEB756ADB7E1E]_file_not_yet_uploaded.txt", It.IsAny<Progress<double>>()), Times.Once);
            fileNotYetUploaded.RemoteId.ShouldSerializeSameAs(remoteId);
        }
    }
}