using Bitretsmah.Core;
using Bitretsmah.Core.Exceptions;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class AccountServiceShould
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ICredentialVerifier> _credentialVerifierMock;
        private IAccountService _accountService;

        [SetUp]
        public void SetUp()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _credentialVerifierMock = new Mock<ICredentialVerifier>();
            _accountService = new AccountService(_accountRepositoryMock.Object, _credentialVerifierMock.Object);
        }

        [Test]
        public async Task GetAllAccounts()
        {
            var accounts = new List<Account>() { new Account() };
            _accountRepositoryMock.Setup(x => x.GetAll()).ReturnsAsync(accounts);

            var result = await _accountService.GetAll();

            _accountRepositoryMock.Verify(x => x.GetAll(), Times.Once());
            result.ShouldBeEquivalentTo(accounts);
        }

        [Test]
        public async Task SetCorrectCredentialsInProperAccount()
        {
            var credential = new NetworkCredential("user@server.com", "Pa$$w0rd");
            _credentialVerifierMock.Setup(x => x.Verify(credential)).ReturnsAsync(true);

            await _accountService.SetCredential(credential);

            _credentialVerifierMock.Verify(x => x.Verify(credential), Times.Once);
            _accountRepositoryMock.Verify(x => x.AddOrUpdate(credential), Times.Once);
        }

        [Test]
        public void ThrowExceptionForIncorrectCredential()
        {
            var credential = new NetworkCredential("user@server.com", "Pa$$w0rd");
            _credentialVerifierMock.Setup(x => x.Verify(credential)).ReturnsAsync(false);

            Assert.That(() => _accountService.SetCredential(credential), Throws.TypeOf<InvalidCredentialException>());

            _credentialVerifierMock.Verify(x => x.Verify(credential), Times.Once);
            _accountRepositoryMock.Verify(x => x.AddOrUpdate(It.IsAny<NetworkCredential>()), Times.Never);
        }
    }
}