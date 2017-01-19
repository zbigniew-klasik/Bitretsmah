using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using Bitretsmah.UI.ConsoleApp;
using Moq;

namespace Bitretsmah.Tests.Unit.ConsoleApp
{
    [TestFixture]
    public class ExecutorShould
    {
        private Mock<IConsoleService> _consoleServiceMock;
        private Mock<ITargetService> _targetServiceMock;
        private IExecutor _executor;

        [SetUp]
        public void SetUp()
        {
            _consoleServiceMock = new Mock<IConsoleService>();
            _targetServiceMock = new Mock<ITargetService>();
            _executor = new Executor(_consoleServiceMock.Object, _targetServiceMock.Object);
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