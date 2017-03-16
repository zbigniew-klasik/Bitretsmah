using Bitretsmah.UI.ConsoleApp;
using FluentAssertions;
using NUnit.Framework;
using System;
using Bitretsmah.Core.Interfaces;
using Moq;

namespace Bitretsmah.Tests.Unit.ConsoleApp
{
    [TestFixture]
    public class ConsoleArgumentsParserShould
    {
        private Mock<ILogger> _loggerMock;
        private ConsoleArgumentsParser _parser;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger>();
            _parser = new ConsoleArgumentsParser(_loggerMock.Object);
        }

        [TestCase("--accounts", true)]
        [TestCase("--targets", false)]
        [TestCase("--accounts --targets", true)]
        [TestCase("--targets --accounts", true)]
        public void ParseAccountsArgument(string arguments, bool expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Accounts.Should().Be(expectedResult);
        }

        [TestCase("", null)]
        [TestCase("--backup foo", "foo")]
        public void ParseBackupArgument(string arguments, string expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Backup.Should().Be(expectedResult);
        }

        [TestCase("--forced", true)]
        [TestCase("--help", false)]
        [TestCase("--forced --help", true)]
        [TestCase("--version --forced", true)]
        public void ParseForceArgument(string arguments, bool expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Forced.Should().Be(expectedResult);
        }

        [TestCase("--forced", false)]
        [TestCase("--help", true)]
        [TestCase("--forced --help", true)]
        [TestCase("--help --version", true)]
        public void ParseHelpArgument(string arguments, bool expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Help.Should().Be(expectedResult);
        }

        [TestCase("", null)]
        [TestCase("--password Pa$$w0rd", "Pa$$w0rd")]
        [TestCase("--set-account john@snow.org --password Pa$$w0rd", "Pa$$w0rd")]
        public void ParsePasswordArgument(string arguments, string expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Password.Should().Be(expectedResult);
        }

        [TestCase(@"", null)]
        [TestCase(@"--path D:\temp\foo", @"D:\temp\foo")]
        [TestCase(@"--path C:\temp\bar.txt", @"C:\temp\bar.txt")]
        [TestCase(@"--set-target foo --path D:\something\important", @"D:\something\important")]
        public void ParsePathArgument(string arguments, string expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Path.Should().Be(expectedResult);
        }

        [TestCase("", null)]
        [TestCase("--restore foo", "foo")]
        public void ParseRestoreArgument(string arguments, string expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Restore.Should().Be(expectedResult);
        }

        [TestCase("", null)]
        [TestCase("--set-account john@snow.org", "john@snow.org")]
        [TestCase("--set-account john@snow.org --password Pa$$w0rd", "john@snow.org")]
        public void ParseSetAccountArgument(string arguments, string expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).SetAccount.Should().Be(expectedResult);
        }

        [TestCase(@"", null)]
        [TestCase(@"--set-target foo", "foo")]
        [TestCase(@"--set-target bar --path D:\temp\bar", "bar")]
        public void ParseSetTargetArgument(string arguments, string expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).SetTarget.Should().Be(expectedResult);
        }

        [TestCase(@"", null)]
        [TestCase(@"--remove-target foo", "foo")]
        public void ParseRemoveTargetArgument(string arguments, string expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).RemoveTarget.Should().Be(expectedResult);
        }

        [TestCase("--accounts", false)]
        [TestCase("--targets", true)]
        [TestCase("--accounts --targets", true)]
        [TestCase("--targets --accounts", true)]
        public void ParseTargetsArgument(string arguments, bool expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Targets.Should().Be(expectedResult);
        }

        [TestCase("--version", true)]
        [TestCase("--help", false)]
        [TestCase("--version --help", true)]
        [TestCase("--forced --version", true)]
        public void ParseVersionArgument(string arguments, bool expectedResult)
        {
            _parser.Parse(arguments.Split(' ')).Version.Should().Be(expectedResult);
        }

        [TestCase("--backup")]
        [TestCase("--restore")]
        [TestCase("--set-account")]
        [TestCase("--password")]
        [TestCase("--set-target")]
        [TestCase("--path")]
        public void ThrowExceptionForParseError(string arguments)
        {
            Assert.Throws<ArgumentException>(() => _parser.Parse(arguments.Split(' ')));
        }

        [Test]
        public void ThrowExceptionForUnknownArgument()
        {
            var ex = Assert.Throws<ArgumentException>(() => _parser.Parse("--foo".Split(' ')));
            ex.Message.Should().Be("Unknown argument: foo.");
        }

        [TestCase(@"--set-target bar --path D:\temp\bar")]
        [TestCase(@"--path D:\temp\bar --set-target bar")]
        public void LogInputArgumentsInSameOrder(string arguments)
        {
            _parser.Parse(arguments.Split(' '));
            _loggerMock.Verify(x => x.Info("Run with arguments: {0}.", arguments), Times.Once);
        }

        [TestCase("--set-account john@snow.org --password Pa$$w0rd", "--set-account john@snow.org --password **********")]
        [TestCase("--password Password-01 --set-account john@snow.org", "--password ********** --set-account john@snow.org")]
        public void LogInputArgumentWithMaskedPassword(string inputArguments, string loggedArguments)
        {
            _parser.Parse(inputArguments.Split(' '));
            _loggerMock.Verify(x => x.Info(It.IsAny<string>(), loggedArguments), Times.Once);
        }
    }
}