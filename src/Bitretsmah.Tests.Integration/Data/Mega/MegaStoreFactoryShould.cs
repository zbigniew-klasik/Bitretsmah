using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Bitretsmah.Data.Mega;
using Moq;
using NUnit.Framework;
using FluentAssertions;

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

        public async Task GetAll()
        {
            var stores = await _remoteFileStoreFactory.GetAll();

            stores.Count.Should().Be(2);
            //stores[0]...
        }
    }
}
