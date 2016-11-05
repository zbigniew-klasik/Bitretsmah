using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
            public RemoteFileWarehouseWrapper(IAccountService accountService, IRemoteFileStoreFactory remoteFileStoreFactory)
                : base(accountService, remoteFileStoreFactory)
            {
            }

            public IList<IRemoteFileStore> RemoteFileStores => _remoteFileStores;
        }

        private class TestStore : IRemoteFileStore
        {
            public TestStore(string storeId, Quota quota)
            {
                TestId = Guid.NewGuid();
                StoreId = storeId;
                Quota = quota;
            }

            public Guid TestId { get; private set; }
            public string StoreId { get; }
            public Quota Quota { get; }

            public Task<RemoteId> UploadFile(string localFilePath)
            {
                throw new NotImplementedException();
            }

            public Task DownloadFile(RemoteId remoteId, string localFilePath)
            {
                throw new NotImplementedException();
            }
        }

        #endregion PRIVATE TESTING CLASSES

        private Mock<IAccountService> _accountServiceMock;
        private Mock<IRemoteFileStoreFactory> _remoteFileStoreFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _accountServiceMock = new Mock<IAccountService>();
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

            var remoteFileWarehouse = new RemoteFileWarehouseWrapper(_accountServiceMock.Object, _remoteFileStoreFactoryMock.Object);
            await remoteFileWarehouse.LoadStores();
            remoteFileWarehouse.RemoteFileStores.ShouldAllBeEquivalentTo(stores);
        }

        [Test]
        public async Task LoadNewStores()
        {
            var remoteFileWarehouse = new RemoteFileWarehouseWrapper(_accountServiceMock.Object, _remoteFileStoreFactoryMock.Object);

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

            actualIds.ShouldBeEquivalentTo(expectedIds);
        }

        [Test]
        public async Task UploadFile()
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
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
                storeMock.SetupGet(x => x.Quota).Returns(new Quota(50, 5 * i));
                stores.Add(storeMock.Object);
            }

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(stores);

            var warehouse = new RemoteFileWarehouse(_accountServiceMock.Object, _remoteFileStoreFactoryMock.Object);
            await warehouse.LoadStores();
            await warehouse.DownloadFile(remoteId, "test_file_path");

            Mock.Get(stores.Single(x => x.StoreId.Equals(storeId))).Verify(x => x.DownloadFile(remoteId, "test_file_path"), Times.Once);
            stores.Where(x => !x.StoreId.Equals(remoteId.StoreId)).ToList().ForEach(x =>
            {
                Mock.Get(x).Verify(y => y.DownloadFile(It.IsAny<RemoteId>(), It.IsAny<string>()), Times.Never);
            });
        }
    }
}