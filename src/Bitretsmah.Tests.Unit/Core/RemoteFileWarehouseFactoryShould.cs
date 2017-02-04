using Bitretsmah.Core;
using Moq;
using NUnit.Framework;
using Bitretsmah.Core.Interfaces;
using FluentAssertions;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class RemoteFileWarehouseFactoryShould
    {
        [Test] //TODO: fix the test
        public void Create()
        {
            var remoteFileStoreFactoryMock = new Mock<IRemoteFileStoreFactory>();
            var remoteFileWarehouseFactory = new RemoteFileWarehouseFactory(remoteFileStoreFactoryMock.Object);
            var remoteFileWarehouse = remoteFileWarehouseFactory.Create();
            remoteFileWarehouse.Should().BeOfType<RemoteFileWarehouse>();
        }
    }
}