﻿using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Moq;
using NUnit.Framework;
using System;
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
            var remoteFileWarehouse = new Mock<IRemoteFileWarehouse>();
            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            remoteFileWarehouseFactoryMock.Setup(x => x.Create()).Returns(remoteFileWarehouse.Object);

            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                                            CreateFile("file", NodeState.Created));

            IChangedFilesUploader changedFilesUploader = new ChangedFilesUploader(hashServiceMock.Object, remoteFileWarehouseFactoryMock.Object);
            await changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            remoteFileWarehouseFactoryMock.Verify(x => x.Create(), Times.Once);
        }

        [Test]
        public async Task NotCreateWarehouseWhenNothingToUpload()
        {
            var hashServiceMock = new Mock<IHashService>();
            var remoteFileWarehouse = new Mock<IRemoteFileWarehouse>();
            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            remoteFileWarehouseFactoryMock.Setup(x => x.Create()).Returns(remoteFileWarehouse.Object);

            var filesStructureChange = CreateDirectory("root", NodeState.Modified,
                                            CreateFile("file", NodeState.Deleted));

            IChangedFilesUploader changedFilesUploader = new ChangedFilesUploader(hashServiceMock.Object, remoteFileWarehouseFactoryMock.Object);
            await changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            remoteFileWarehouseFactoryMock.Verify(x => x.Create(), Times.Never);
        }

        [Test]
        public async Task ComputeHashForEachFileWithEmptyHash()
        {
            var hashServiceMock = new Mock<IHashService>();
            var remoteFileWarehouse = new Mock<IRemoteFileWarehouse>();
            var remoteFileWarehouseFactoryMock = new Mock<IRemoteFileWarehouseFactory>();
            remoteFileWarehouseFactoryMock.Setup(x => x.Create()).Returns(remoteFileWarehouse.Object);

            var filesStructureChange = CreateDirectory("directory1", NodeState.Modified,
                                            CreateFile("file1", NodeState.Created, "any hash"),
                                            CreateFile("file2", NodeState.Modified, string.Empty),
                                            CreateFile("file3", NodeState.Deleted, null),
                                            CreateDirectory("directory2", NodeState.Modified,
                                                CreateFile("file4", NodeState.Created, null),
                                                CreateFile("file5", NodeState.Modified, "any hash"),
                                                CreateFile("file6", NodeState.Deleted, string.Empty)));

            IChangedFilesUploader changedFilesUploader = new ChangedFilesUploader(hashServiceMock.Object, remoteFileWarehouseFactoryMock.Object);
            await changedFilesUploader.Upload(filesStructureChange, new Progress<BackupProgress>());

            hashServiceMock.Verify(x => x.ComputeFileHash(It.IsAny<string>()), Times.Exactly(2));
            hashServiceMock.Verify(x => x.ComputeFileHash(@"C:\Temp\file2.txt"), Times.Once);
            hashServiceMock.Verify(x => x.ComputeFileHash(@"C:\Temp\file4.txt"), Times.Once);
        }
    }
}