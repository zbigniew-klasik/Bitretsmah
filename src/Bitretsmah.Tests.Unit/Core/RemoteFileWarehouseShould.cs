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
        }

        #endregion PRIVATE TESTING CLASSES

        private Mock<IAccountService> _accountServiceMock;
        private Mock<IRemoteFileStoreFactory> _remoteFileStoreFactoryMock;
        private IRemoteFileWarehouse _remoteFileWarehouse;
        private List<IRemoteFileStore> _stores;

        [SetUp]
        public void SetUp()
        {
            _accountServiceMock = new Mock<IAccountService>();

            _stores = new List<IRemoteFileStore>()
            {
                new TestStore("store1", new Quota(100, 15)),
                new TestStore("store2", new Quota(100, 10)),
                new TestStore("store3", new Quota(100, 0)),
                new TestStore("store4", new Quota(100, 90)),
                new TestStore("store5", new Quota(100, 60))
            };

            _remoteFileStoreFactoryMock = new Mock<IRemoteFileStoreFactory>();
            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(_stores);

            _remoteFileWarehouse = new RemoteFileWarehouseWrapper(_accountServiceMock.Object, _remoteFileStoreFactoryMock.Object);
        }

        [Test]
        public async Task LoadStores()
        {
            await _remoteFileWarehouse.LoadStores();

            ((RemoteFileWarehouseWrapper)_remoteFileWarehouse)
                .RemoteFileStores
                .ShouldAllBeEquivalentTo(_stores);
        }

        [Test]
        public async Task LoadNewStores()
        {
            var oldStoreList = new List<IRemoteFileStore>()
            {
                new TestStore("storeA", new Quota()),
                new TestStore("storeB", new Quota()),
            };

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(oldStoreList);

            await _remoteFileWarehouse.LoadStores();

            var newStoreList = new List<IRemoteFileStore>()
            {
                new TestStore("storeA", new Quota()),
                new TestStore("storeB", new Quota()),
                new TestStore("storeC", new Quota()),
            };

            _remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(newStoreList);

            await _remoteFileWarehouse.LoadStores();

            var expectedStoreList = new List<IRemoteFileStore>();
            expectedStoreList.Add(oldStoreList[0]);
            expectedStoreList.Add(oldStoreList[1]);
            expectedStoreList.Add(newStoreList[2]);

            var expectedIds = expectedStoreList.Select(x => (x as TestStore).TestId).ToList();

            var actualStoreList = ((RemoteFileWarehouseWrapper)_remoteFileWarehouse).RemoteFileStores;

            var actualIds = actualStoreList.Select(x => (x as TestStore).TestId).ToList();

            actualIds.ShouldBeEquivalentTo(expectedIds);
        }

        [Test]
        public async Task UploadFile()
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }

        [Test]
        public async Task DownloadFile()
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }
    }
}