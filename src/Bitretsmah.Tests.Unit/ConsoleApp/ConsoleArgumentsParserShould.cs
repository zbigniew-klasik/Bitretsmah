using System;
using Bitretsmah.UI.ConsoleApp;
using FluentAssertions;
using NUnit.Framework;

namespace Bitretsmah.Tests.Unit.ConsoleApp
{
    [TestFixture]
    public class ConsoleArgumentsParserShould
    {
        private ConsoleArguments Parse(string arguments)
        {
            string[] args = arguments.Split(' ');
            var parser = new ConsoleArgumentsParser();
            return parser.Parse(args);
        }

        [TestCase("", null)]
        [TestCase("--backup foo", "foo")]
        public void ParseBackupArgument(string arguments, string expectedResult)
        {
            Parse(arguments).Backup.Should().Be(expectedResult);
        }

        [TestCase("", null)]
        [TestCase("--restore foo", "foo")]
        public void ParseRestoreArgument(string arguments, string expectedResult)
        {
            Parse(arguments).Restore.Should().Be(expectedResult);
        }

        [TestCase("", null)]
        [TestCase("--set-account john@snow.org", "john@snow.org")]
        [TestCase("--set-account john@snow.org --password Pa$$w0rd", "john@snow.org")]
        public void ParseSetAccountArgument(string arguments, string expectedResult)
        {
            Parse(arguments).SetAccount.Should().Be(expectedResult);
        }

        [TestCase("", null)]
        [TestCase("--password Pa$$w0rd", "Pa$$w0rd")]
        [TestCase("--set-account john@snow.org --password Pa$$w0rd", "Pa$$w0rd")]
        public void ParsePasswordArgument(string arguments, string expectedResult)
        {
            Parse(arguments).Password.Should().Be(expectedResult);
        }

        [TestCase(@"", null)]
        [TestCase(@"--set-target foo", "foo")]
        [TestCase(@"--set-target bar --path D:\temp\bar", "bar")]
        public void ParseSetTargetArgument(string arguments, string expectedResult)
        {
            Parse(arguments).SetTarget.Should().Be(expectedResult);
        }

        [TestCase(@"", null)]
        [TestCase(@"--path D:\temp\foo", @"D:\temp\foo")]
        [TestCase(@"--path C:\temp\bar.txt", @"C:\temp\bar.txt")]
        [TestCase(@"--set-target foo --path D:\something\important", @"D:\something\important")]
        public void ParsePathArgument(string arguments, string expectedResult)
        {
            Parse(arguments).Path.Should().Be(expectedResult);
        }

        [TestCase("--forced", true)]
        [TestCase("--help", false)]
        [TestCase("--forced --help", true)]
        [TestCase("--version --forced", true)]
        public void ParseForceArgument(string arguments, bool expectedResult)
        {
            Parse(arguments).Forced.Should().Be(expectedResult);
        }

        [TestCase("--forced", false)]
        [TestCase("--help", true)]
        [TestCase("--forced --help", true)]
        [TestCase("--help --version", true)]
        public void ParseHelpArgument(string arguments, bool expectedResult)
        {
            Parse(arguments).Help.Should().Be(expectedResult);
        }

        [TestCase("--version", true)]
        [TestCase("--help", false)]
        [TestCase("--version --help", true)]
        [TestCase("--forced --version", true)]
        public void ParseVersionArgument(string arguments, bool expectedResult)
        {
            Parse(arguments).Version.Should().Be(expectedResult);
        }

        [TestCase("--backup")]
        [TestCase("--restore")]
        [TestCase("--set-account")]
        [TestCase("--password")]
        [TestCase("--set-target")]
        [TestCase("--path")]
        public void ThrowExceptionForParseError(string arguments)
        {
            Assert.Throws<ArgumentException>(() => Parse(arguments));
        }

        [Test]
        public void ThrowExceptionForUnknownArgument()
        {
            var ex = Assert.Throws<ArgumentException>(() => Parse("--foo"));
            ex.Message.Should().Be("Unknown argument: foo.");
        }
    }
}