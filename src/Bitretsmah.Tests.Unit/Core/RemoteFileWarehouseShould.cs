using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class RemoteFileWarehouseShould
    {
        private Mock<IAccountService> _accountServiceMock;
        private Mock<IRemoteFileStoreFactory> _remoteFileStoreFactoryMock;
        private IRemoteFileWarehouse _remoteFileWarehouse;

        [SetUp]
        public void SetUp()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _remoteFileStoreFactoryMock = new Mock<IRemoteFileStoreFactory>();
            _remoteFileWarehouse = new RemoteFileWarehouse(_accountServiceMock.Object, _remoteFileStoreFactoryMock.Object);
        }

        [Test]
        public async Task LoadStores()
        {
            var x = await Task.FromResult(0);
            x.Should().Be(0);
            throw new NotImplementedException();
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