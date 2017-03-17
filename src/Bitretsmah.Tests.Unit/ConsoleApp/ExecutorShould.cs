using System;
using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using Bitretsmah.UI.ConsoleApp;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bitretsmah.Core.Exceptions;
using Bitretsmah.Core.Interfaces;
using FluentAssertions;

namespace Bitretsmah.Tests.Unit.ConsoleApp
{
    [TestFixture]
    public class ExecutorShould
    {
        private Mock<IAccountService> _accountServiceMock;
        private Mock<IBackupService> _backupServiceMock;
        private Mock<IConsoleService> _consoleServiceMock;
        private Mock<ILogger> _loggerMock;
        private Mock<ITargetService> _targetServiceMock;
        private IExecutor _executor;

        [SetUp]
        public void SetUp()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _backupServiceMock = new Mock<IBackupService>();
            _consoleServiceMock = new Mock<IConsoleService>();
            _loggerMock = new Mock<ILogger>();
            _targetServiceMock = new Mock<ITargetService>();
            _executor = new Executor(_accountServiceMock.Object, _backupServiceMock.Object, _consoleServiceMock.Object, _loggerMock.Object, _targetServiceMock.Object);
        }

        #region Help & Version

        [Test]
        public async Task WritesHelp()
        {
            var arguments = new ConsoleArguments
            {
                Help = true
            };

            await _executor.Execut(arguments);

            _consoleServiceMock.Verify(x => x.WriteHelp());
        }

        [Test]
        public async Task WritesHelpForEmptyArguments()
        {
            var arguments = new ConsoleArguments();

            await _executor.Execut(arguments);

            _consoleServiceMock.Verify(x => x.WriteHelp());
        }

        [Test]
        public async Task WritesVersion()
        {
            var arguments = new ConsoleArguments
            {
                Version = true
            };

            await _executor.Execut(arguments);

            _consoleServiceMock.Verify(x => x.WriteVersion());
        }

        #endregion Help & Version

        #region Accounts

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

        #endregion Accounts

        #region Targets

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
        public async Task RemoveTarget()
        {
            var arguments = new ConsoleArguments
            {
                RemoveTarget = "Target Name"
            };

            await _executor.Execut(arguments);

            _targetServiceMock.Verify(x => x.RemoveTarget("Target Name"));
            _consoleServiceMock.Verify(x => x.TargetRemovedSuccessfully());
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

        #endregion Targets

        #region Exceptions

        [Test]
        public async Task HandleBitretsmahException()
        {
            var ex = new BitretsmahException("expected exception");
            _targetServiceMock.Setup(x => x.GetAll()).ThrowsAsync(ex);

            var arguments = new ConsoleArguments { Targets = true };
            await _executor.Execut(arguments);

            _loggerMock.Verify(x => x.Warn(ex));
            _consoleServiceMock.Verify(x => x.WriteErrorMessage("expected exception"), Times.Once);
        }

        [Test]
        public async Task HandleUnexpectedException()
        {
            var ex = new Exception("unexpected exception");
            _targetServiceMock.Setup(x => x.GetAll()).ThrowsAsync(ex);

            var arguments = new ConsoleArguments { Targets = true };
            await _executor.Execut(arguments);

            _loggerMock.Verify(x => x.Error(ex));
            _consoleServiceMock.Verify(x => x.WriteUnexpectedException(ex), Times.Once);
        }

        #endregion Exceptions

        #region Backup & Restore

        [Test]
        public async Task Backup()
        {
            BackupRequest request = null;
            _backupServiceMock.Setup(x => x.Backup(It.IsAny<BackupRequest>())).Callback<BackupRequest>(x => request = x);

            var arguments = new ConsoleArguments
            {
                Backup = "Target Name"
            };

            await _executor.Execut(arguments);

            _backupServiceMock.Verify(x => x.Backup(It.IsAny<BackupRequest>()));
            request.Should().NotBeNull();
            request.TargetName.Should().Be("Target Name");
            request.Progress.Should().NotBeNull();
            request.ComputeHashForEachFile.Should().BeFalse();
        }

        [Test]
        public async Task BackupWithHashAllFiles()
        {
            BackupRequest request = null;
            _backupServiceMock.Setup(x => x.Backup(It.IsAny<BackupRequest>())).Callback<BackupRequest>(x => request = x);

            var arguments = new ConsoleArguments
            {
                Backup = "Target Name",
                Hash = true
            };

            await _executor.Execut(arguments);

            request.Should().NotBeNull();
            request.ComputeHashForEachFile.Should().BeTrue();
        }

        [Test]
        public async Task Restore()
        {
            RestoreRequest request = null;
            _backupServiceMock.Setup(x => x.Restore(It.IsAny<RestoreRequest>())).Callback<RestoreRequest>(x => request = x);

            var arguments = new ConsoleArguments
            {
                Restore = "Target Name"
            };

            await _executor.Execut(arguments);

            _backupServiceMock.Verify(x => x.Restore(It.IsAny<RestoreRequest>()));
            request.Should().NotBeNull();
            request.TargetName.Should().Be("Target Name");
            request.Progress.Should().NotBeNull();
            request.ComputeHashForEachFile.Should().BeFalse();
        }

        [Test]
        public async Task RestoreWithHashAllFiles()
        {
            RestoreRequest request = null;
            _backupServiceMock.Setup(x => x.Restore(It.IsAny<RestoreRequest>())).Callback<RestoreRequest>(x => request = x);

            var arguments = new ConsoleArguments
            {
                Restore = "Target Name",
                Hash = true
            };

            await _executor.Execut(arguments);

            request.Should().NotBeNull();
            request.ComputeHashForEachFile.Should().BeTrue();
        }

        #endregion
    }
}