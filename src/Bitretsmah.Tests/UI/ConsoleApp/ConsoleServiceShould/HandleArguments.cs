using Bitretsmah.Core;
using Bitretsmah.UI.ConsoleApp;
using Moq;
using NUnit.Framework;
using System.Net;

namespace Bitretsmah.Tests.UI.ConsoleApp.ConsoleServiceShould
{
    [TestFixture]
    public class HandleArgumentsShould
    {
        private Mock<IAccountService> _accountServiceMock;
        private IConsoleService _consoleService;

        [SetUp]
        public void SetUp()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _consoleService = new ConsoleService(_accountServiceMock.Object);
        }

        [Test]
        public void AddNewAccount()
        {
            var args = new string[] { "--set-account", "test@server.com", "Pa$$w0rd" };
            _consoleService.HandleArguments(args);
            _accountServiceMock.Verify(x => x.SetCredential(It.IsAny<NetworkCredential>()), Times.Once());
        }
    }
}