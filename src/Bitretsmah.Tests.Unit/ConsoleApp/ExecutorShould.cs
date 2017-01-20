using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using Bitretsmah.UI.ConsoleApp;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;

namespace Bitretsmah.Tests.Unit.ConsoleApp
{
    [TestFixture]
    public class ExecutorShould
    {
        private Mock<IAccountService> _accountServiceMock;
        private Mock<IConsoleService> _consoleServiceMock;
        private Mock<ITargetService> _targetServiceMock;
        private IExecutor _executor;

        [SetUp]
        public void SetUp()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _consoleServiceMock = new Mock<IConsoleService>();
            _targetServiceMock = new Mock<ITargetService>();
            _executor = new Executor(_accountServiceMock.Object, _consoleServiceMock.Object, _targetServiceMock.Object);
        }

        [Test]
        public async Task SetAccountWithProvidedPassword()
        {
            NetworkCredential actualCredential = null;
            _accountServiceMock
                .Setup(x => x.SetCredential(It.IsAny<NetworkCredential>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkCredential>(x => actualCredential = x);

            var arguments = new ConsoleArguments
            {
                SetAccount = "test@account.com",
                Password = "Pa$$w0rd"
            };

            await _executor.Execut(arguments);

            _accountServiceMock.Verify(x => x.SetCredential(It.IsAny<NetworkCredential>()));
            actualCredential.UserName.Should().Be("test@account.com");
            actualCredential.Password.Should().Be("Pa$$w0rd");
            _consoleServiceMock.Verify(x => x.AccountSetSuccessfully());
        }

        [Test]
        public async Task SetAccountWithoutProvidedPassword()
        {
            NetworkCredential actualCredential = null;
            _accountServiceMock
                .Setup(x => x.SetCredential(It.IsAny<NetworkCredential>()))
                .Returns(Task.CompletedTask)
                .Callback<NetworkCredential>(x => actualCredential = x);

            _consoleServiceMock
                .Setup(x => x.ReadPassword())
                .Returns(new NetworkCredential("", "Pa$$w0rd").SecurePassword);

            var arguments = new ConsoleArguments
            {
                SetAccount = "test@account.com"
            };

            await _executor.Execut(arguments);

            _consoleServiceMock.Verify(x => x.ReadPassword());
            _accountServiceMock.Verify(x => x.SetCredential(It.IsAny<NetworkCredential>()));
            actualCredential.UserName.Should().Be("test@account.com");
            actualCredential.Password.Should().Be("Pa$$w0rd");
            _consoleServiceMock.Verify(x => x.AccountSetSuccessfully());
        }

        [Test]
        public async Task ListAllAccounts()
        {
            var accounts = new List<Account>() { new Account(), new Account() };
            _accountServiceMock.Setup(x => x.GetAll()).ReturnsAsync(accounts);

            var arguments = new ConsoleArguments
            {
                Accounts = true
            };

            await _executor.Execut(arguments);

            _accountServiceMock.Verify(x => x.GetAll());
            _consoleServiceMock.Verify(x => x.ListAccounts(accounts));
        }

        [Test]
        public async Task SetTarget()
        {
            var arguments = new ConsoleArguments
            {
                SetTarget = "Target Name",
                Path = @"C:\path"
            };

            await _executor.Execut(arguments);

            _targetServiceMock.Verify(x => x.SetTarget("Target Name", @"C:\path"));
            _consoleServiceMock.Verify(x => x.TargetSetSuccessfully());
        }

        [Test]
        public async Task ListAllTargets()
        {
            var targets = new List<Target>() { new Target(), new Target() };
            _targetServiceMock.Setup(x => x.GetAll()).ReturnsAsync(targets);

            var arguments = new ConsoleArguments
            {
                Targets = true
            };

            await _executor.Execut(arguments);

            _targetServiceMock.Verify(x => x.GetAll());
            _consoleServiceMock.Verify(x => x.ListTargets(targets));
        }
    }
}