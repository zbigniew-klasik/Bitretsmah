using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class RemoteFileWarehouseShould
    {
        #region PRIVATE TESTING CLASSES

        private class RemoteFileWarehouseWrapper : RemoteFileWarehouse
        {
            public RemoteFileWarehouseWrapper(IRemoteFileStoreFactory remoteFileStoreFactory)
                : base(remoteFileStoreFactory)
            {
            }

            public IList<IRemoteFileStore> RemoteFileStores => _remoteFileStores;
        }

        private class TestStore : IRemoteFileStore
        {
            private readonly Quota _quota;

            public TestStore(string storeId, Quota quota)
            {
                TestId = Guid.Empty;
                StoreId = storeId;
                _quota = quota;
            }

            public Guid TestId { get; private set; }
            public string StoreId { get; }

            public Task<Quota> GetQuota()
            {
                return Task.FromResult(_quota);
            }

            public Task<ICollection<RemoteFile>> GetFilesList() => null;

            public Task<RemoteId> UploadFile(Stream stream, string remoteFileName, IProgress<double> progress) => null;

            public Task<Stream> DownloadFile(RemoteId remoteId, IProgress<double> progress) => null;

            public void Dispose() => Console.WriteLine("dispose");
        }

        #endregion PRIVATE TESTING CLASSES

        private Mock<IRemoteFileStoreFactory> _remoteFileStoreFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _remoteFileStoreFactoryMock = new Mock<IRemoteFileStoreFactory>();
        }

        [Test]
        public async Task LoadStores()
        {
            var stores = new List<IRemoteFileStore>()
            {
                new TestStore("store1", new Quota(100, 15)),
                new TestStore("store2", new Quota(100, 10)),
                new TestStore("store3", new Quota(100, 0)),
            };

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(stores);

            var remoteFileWarehouse = new RemoteFileWarehouseWrapper(_remoteFileStoreFactoryMock.Object);
            await remoteFileWarehouse.LoadStores();
            remoteFileWarehouse.RemoteFileStores.ShouldSerializeSameAs(stores);
        }

        [Test]
        public async Task LoadNewStores()
        {
            var remoteFileWarehouse = new RemoteFileWarehouseWrapper(_remoteFileStoreFactoryMock.Object);

            var oldStoreList = new List<IRemoteFileStore>()
            {
                new TestStore("storeA", new Quota()),
                new TestStore("storeB", new Quota()),
            };

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(oldStoreList);

            await remoteFileWarehouse.LoadStores();

            var newStoreList = new List<IRemoteFileStore>()
            {
                new TestStore("storeA", new Quota()),
                new TestStore("storeB", new Quota()),
                new TestStore("storeC", new Quota()),
            };

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(newStoreList);

            await remoteFileWarehouse.LoadStores();

            var expectedStoreList = new List<IRemoteFileStore>();
            expectedStoreList.Add(oldStoreList[0]);
            expectedStoreList.Add(oldStoreList[1]);
            expectedStoreList.Add(newStoreList[2]);

            var expectedIds = expectedStoreList.Select(x => (x as TestStore).TestId).ToList();

            var actualStoreList = remoteFileWarehouse.RemoteFileStores;

            var actualIds = actualStoreList.Select(x => (x as TestStore).TestId).ToList();

            actualIds.ShouldSerializeSameAs(expectedIds);
        }

        [Test]
        public async Task DisposeAllStores()
        {
            var mocks = new List<Mock<IRemoteFileStore>>();
            var stores = new List<IRemoteFileStore>();

            for (int i = 0; i < 10; i++)
            {
                var mock = new Mock<IRemoteFileStore>();
                mock.Setup(x => x.StoreId).Returns(i.ToString());
                mocks.Add(mock);
                stores.Add(mock.Object);
            }

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(stores);

            using (var remoteFileWarehouse = new RemoteFileWarehouse(_remoteFileStoreFactoryMock.Object))
            {
                await remoteFileWarehouse.LoadStores();
            }

            mocks.ForEach(x => x.Verify(y => y.Dispose()));
        }

        [TestCase(StoreSelectionMethod.WithLessFreeSpace, "store_9")]
        [TestCase(StoreSelectionMethod.WithMoreFreeSpace, "store_0")]
        public async Task UploadFileToCorrectStore(StoreSelectionMethod method, string storeId)
        {
            var stores = new List<IRemoteFileStore>();

            for (int i = 0; i < 10; i++)
            {
                var storeMock = new Mock<IRemoteFileStore>();
                storeMock.SetupGet(x => x.StoreId).Returns($"store_{i}");
                storeMock.Setup(x => x.GetQuota()).ReturnsAsync(new Quota(50, 20 + i));
                storeMock.Setup(x => x.UploadFile(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<IProgress<double>>())).ReturnsAsync(new RemoteId($"store_{i}", "test_node_id"));
                stores.Add(storeMock.Object);
            }

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(stores);

            IRemoteFileWarehouse warehouse = new RemoteFileWarehouse(_remoteFileStoreFactoryMock.Object);
            warehouse.StoreSelectionMethod = method;
            await warehouse.LoadStores();

            var stream = new MemoryStream();
            var progress = new Progress<double>();
            var remoteId = await warehouse.UploadFile(stream, "remote_file_name", progress);

            remoteId.StoreId.Should().Be(storeId);
            Mock.Get(stores.Single(x => x.StoreId.Equals(storeId))).Verify(x => x.UploadFile(stream, "remote_file_name", progress), Times.Once);
            stores.Where(x => !x.StoreId.Equals(storeId)).ToList().ForEach(x =>
            {
                Mock.Get(x).Verify(y => y.UploadFile(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<IProgress<double>>()), Times.Never);
            });
        }

        [TestCase("store_3")]
        [TestCase("store_5")]
        [TestCase("store_8")]
        public async Task DownloadFileFromCorrectStore(string storeId)
        {
            var remoteId = new RemoteId(storeId, "test_node_id");
            var stores = new List<IRemoteFileStore>();

            for (int i = 0; i < 10; i++)
            {
                var storeMock = new Mock<IRemoteFileStore>();
                storeMock.SetupGet(x => x.StoreId).Returns($"store_{i}");
                storeMock.Setup(x => x.GetQuota()).ReturnsAsync(new Quota(50, 5 * i));
                stores.Add(storeMock.Object);
            }

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(stores);

            IRemoteFileWarehouse warehouse = new RemoteFileWarehouse(_remoteFileStoreFactoryMock.Object);
            await warehouse.LoadStores();
            var progress = new Progress<double>();
            await warehouse.DownloadFile(remoteId, progress);

            Mock.Get(stores.Single(x => x.StoreId.Equals(storeId))).Verify(x => x.DownloadFile(remoteId, progress), Times.Once);
            stores.Where(x => !x.StoreId.Equals(remoteId.StoreId)).ToList().ForEach(x =>
            {
                Mock.Get(x).Verify(y => y.DownloadFile(It.IsAny<RemoteId>(), It.IsAny<IProgress<double>>()), Times.Never);
            });
        }

        [Test]
        public async Task GetFilesListFromAllStores()
        {
            var stores = new List<IRemoteFileStore>();

            for (int i = 0; i < 3; i++)
            {
                var storeMock = new Mock<IRemoteFileStore>();
                storeMock.SetupGet(x => x.StoreId).Returns($"store_{i}");
                storeMock.Setup(x => x.GetFilesList())
                    .ReturnsAsync(new List<RemoteFile>()
                {
                    new RemoteFile
                    {
                        Id = new RemoteId($"store_{i}", $"file_{i}0"),
                        Name = $"Abc_{i}.txt",
                        Size = 100 + i
                    },
                    new RemoteFile
                    {
                        Id = new RemoteId($"store_{i}", $"file_{i}1"),
                        Name = $"Cde_{i}.bin",
                        Size = 100 + i
                    }
                });

                stores.Add(storeMock.Object);
            }

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(stores);

            IRemoteFileWarehouse warehouse = new RemoteFileWarehouse(_remoteFileStoreFactoryMock.Object);
            await warehouse.LoadStores();

            var actualFilesList = await warehouse.GetFilesList();

            var expectedFilesList = new List<RemoteFile>()
            {
                new RemoteFile
                {
                    Id = new RemoteId($"store_0", $"file_00"),
                    Name = $"Abc_0.txt",
                    Size = 100
                },
                new RemoteFile
                {
                    Id = new RemoteId($"store_0", $"file_01"),
                    Name = $"Cde_0.bin",
                    Size = 100
                },
                new RemoteFile
                {
                    Id = new RemoteId($"store_1", $"file_10"),
                    Name = $"Abc_1.txt",
                    Size = 101
                },
                new RemoteFile
                {
                    Id = new RemoteId($"store_1", $"file_11"),
                    Name = $"Cde_1.bin",
                    Size = 101
                },
                new RemoteFile
                {
                    Id = new RemoteId($"store_2", $"file_20"),
                    Name = $"Abc_2.txt",
                    Size = 102
                },
                new RemoteFile
                {
                    Id = new RemoteId($"store_2", $"file_21"),
                    Name = $"Cde_2.bin",
                    Size = 102
                }
            };

            actualFilesList.ShouldBeEquivalentTo(expectedFilesList);
            actualFilesList.ShouldSerializeSameAs(expectedFilesList);
        }
    }
}