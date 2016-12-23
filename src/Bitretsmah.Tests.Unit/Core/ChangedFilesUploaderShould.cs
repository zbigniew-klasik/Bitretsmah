using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static Bitretsmah.Tests.Unit.Core.NodesTestHelper;

namespace Bitretsmah.Tests.Unit.Core
{
    public class ChangedFilesUploaderShould
    {
        [Test]
        public async Task CreateWarehouse()
        {
            var hashServiceMock = new Mock<IHashService>();
            var localFilesServiceMock = new Mock<ILocalFilesService>();

            var remoteFileWarehouseMock = new Mock<IRemoteFileWarehouse>();
            remoteFileWarehouseMock.Setup(x => x.GetFilesList()).ReturnsAsync(new List<RemoteFile>());

            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            remoteFileWarehouseFactoryMock.Setup(x => x.Create()).Returns(remoteFileWarehouseMock.Object);

            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                                            CreateFile("file", NodeState.Created));

            IChangedFilesUploader changedFilesUploader = new ChangedFilesUploader(hashServiceMock.Object, localFilesServiceMock.Object, remoteFileWarehouseFactoryMock.Object);
            await changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            remoteFileWarehouseFactoryMock.Verify(x => x.Create(), Times.Once);
            remoteFileWarehouseMock.Verify(x => x.GetFilesList(), Times.Once);
        }

        [Test]
        public async Task NotCreateWarehouseWhenNothingToUpload()
        {
            var hashServiceMock = new Mock<IHashService>();
            var localFilesServiceMock = new Mock<ILocalFilesService>();
            var remoteFileWarehouseMock = new Mock<IRemoteFileWarehouse>();
            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            remoteFileWarehouseFactoryMock.Setup(x => x.Create()).Returns(remoteFileWarehouseMock.Object);

            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                                            CreateFile("file", NodeState.Deleted));

            IChangedFilesUploader changedFilesUploader = new ChangedFilesUploader(hashServiceMock.Object, localFilesServiceMock.Object, remoteFileWarehouseFactoryMock.Object);
            await changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            remoteFileWarehouseFactoryMock.Verify(x => x.Create(), Times.Never);
            remoteFileWarehouseMock.Verify(x => x.GetFilesList(), Times.Never);
        }

        [Test]
        public async Task ComputeHashForEachFileWithEmptyHash()
        {
            var hashServiceMock = new Mock<IHashService>();
            var localFilesServiceMock = new Mock<ILocalFilesService>();

            var remoteFileWarehouseMock = new Mock<IRemoteFileWarehouse>();
            remoteFileWarehouseMock.Setup(x => x.GetFilesList()).ReturnsAsync(new List<RemoteFile>());

            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            remoteFileWarehouseFactoryMock.Setup(x => x.Create()).Returns(remoteFileWarehouseMock.Object);

            var filesStructureChange = CreateDirectory("directory1", NodeState.Modified,
                                            CreateFile("file1.txt", NodeState.Created, "any hash"),
                                            CreateFile("file2.txt", NodeState.Modified, string.Empty),
                                            CreateFile("file3.txt", NodeState.Deleted, null),
                                            CreateDirectory("directory2", NodeState.Modified,
                                                CreateFile("file4.txt", NodeState.Created, null),
                                                CreateFile("file5.txt", NodeState.Modified, "any hash"),
                                                CreateFile("file6.txt", NodeState.Deleted, string.Empty)));

            IChangedFilesUploader changedFilesUploader = new ChangedFilesUploader(hashServiceMock.Object, localFilesServiceMock.Object, remoteFileWarehouseFactoryMock.Object);
            await changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            hashServiceMock.Verify(x => x.ComputeFileHash(It.IsAny<string>()), Times.Exactly(2));
            hashServiceMock.Verify(x => x.ComputeFileHash(@"C:\Temp\file2.txt"), Times.Once);
            hashServiceMock.Verify(x => x.ComputeFileHash(@"C:\Temp\file4.txt"), Times.Once);
        }

        [Test]
        public async Task UploadsEachFileNotUploadedYet()
        {
            var previouslyUploadedFiles = new List<RemoteFile>()
            {
                new RemoteFile { Name = "any_file.bin" },
                new RemoteFile { Name = "[A8499F0A32485CB682D5AB7050F967CFBA504D84]_file_uploaded_before.txt" }
            };

            var hashServiceMock = new Mock<IHashService>();

            var fileStream = new MemoryStream();
            var localFilesServiceMock = new Mock<ILocalFilesService>();
            localFilesServiceMock.Setup(x => x.ReadFileStream(@"C:\Temp\file_not_yet_uploaded.txt")).Returns(fileStream);

            var remoteFileWarehouseMock = new Mock<IRemoteFileWarehouse>();
            remoteFileWarehouseMock.Setup(x => x.GetFilesList()).ReturnsAsync(previouslyUploadedFiles);

            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            remoteFileWarehouseFactoryMock.Setup(x => x.Create()).Returns(remoteFileWarehouseMock.Object);

            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                                            CreateFile("file_uploaded_before.txt", NodeState.Modified, "A8499F0A32485CB682D5AB7050F967CFBA504D84"),
                                            CreateFile("file_not_yet_uploaded.txt", NodeState.Modified, "F7031B995F0F2E9CED6D62156F3DEB756ADB7E1E"));

            IChangedFilesUploader changedFilesUploader = new ChangedFilesUploader(hashServiceMock.Object, localFilesServiceMock.Object, remoteFileWarehouseFactoryMock.Object);
            await changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            remoteFileWarehouseMock.Verify(x => x.UploadFile(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<Progress<double>>()), Times.Once);
            remoteFileWarehouseMock.Verify(x => x.UploadFile(fileStream, "[F7031B995F0F2E9CED6D62156F3DEB756ADB7E1E]_file_not_yet_uploaded.txt", It.IsAny<Progress<double>>()), Times.Once);
        }
    }
}