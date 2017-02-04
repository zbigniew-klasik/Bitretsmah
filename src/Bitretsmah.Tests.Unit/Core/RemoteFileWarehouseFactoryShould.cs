using System.Collections.Generic;
using Bitretsmah.Core;
using Moq;
using NUnit.Framework;
using Bitretsmah.Core.Interfaces;
using FluentAssertions;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class RemoteFileWarehouseFactoryShould
    {
        [Test]
        public async Task Create()
        {
            var remoteFileStoreFactoryMock = new Mock<IRemoteFileStoreFactory>();
            remoteFileStoreFactoryMock.Setup(x => x.GetAll()).ReturnsAsync(new List<IRemoteFileStore>());

            var remoteFileWarehouseFactory = new RemoteFileWarehouseFactory(remoteFileStoreFactoryMock.Object);

            var remoteFileWarehouse = await remoteFileWarehouseFactory.Create();

            remoteFileWarehouse.Should().BeOfType<RemoteFileWarehouse>();
            remoteFileStoreFactoryMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}