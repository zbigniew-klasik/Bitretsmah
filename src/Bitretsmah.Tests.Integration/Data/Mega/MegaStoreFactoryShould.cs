using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Bitretsmah.Data.Mega;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Integration.Data.Mega
{
    [TestFixture]
    public class MegaStoreFactoryShould
    {
        private NetworkCredential _credential1;
        private NetworkCredential _credential2;
        private Mock<IAccountService> _accountServiceMock;
        private IRemoteFileStoreFactory _remoteFileStoreFactory;

        [SetUp]
        public void SetUp()
        {
            _credential1 = new NetworkCredential("user1", "Password-01");
            _credential2 = new NetworkCredential("user2", "Password-02");

            var accounts = new List<Account>()
            {
                new Account { Credential = _credential1},
                new Account { Credential = _credential2}
            };

            _accountServiceMock = new Mock<IAccountService>();
            _accountServiceMock.Setup(x => x.GetAll()).ReturnsAsync(accounts);
            _remoteFileStoreFactory = new MegaStoreFactory(_accountServiceMock.Object);
        }

        [Test]
        public async Task GetAllMegaStoresForConfiguredAccounts()
        {
            var allMegaStores = await _remoteFileStoreFactory.GetAll();

            _accountServiceMock.Verify(x => x.GetAll(), Times.Once);

            allMegaStores.Count.Should().Be(2);
            (allMegaStores[0] as MegaStore).UserName.Should().Be(_credential1.UserName);
            (allMegaStores[1] as MegaStore).UserName.Should().Be(_credential2.UserName);
        }
    }
}