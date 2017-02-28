using NUnit.Framework;
using StructureMap;
using Bitretsmah.UI.ConsoleApp;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Tests.Integration.UI.ConsoleApp
{
    public class ContainerShould
    {
        private Container _container;

        [SetUp]
        public void Setup()
        {
            _container = new Container(ContainerRegistry.CreateCompleteRegistry());
        }

        [Test]
        public void GetExecutor()
        {
            var executor = _container.GetInstance<IExecutor>();
            Assert.IsNotNull(executor);
        }

        [Test]
        public void GetConsoleArgumentsParser()
        {
            var consoleArgumentsParser = _container.GetInstance<IConsoleArgumentsParser>();
            Assert.IsNotNull(consoleArgumentsParser);
        }

        [Test]
        public void GetLogger()
        {
            var logger = _container.GetInstance<ILogger>();
            Assert.IsNotNull(logger);
        }
    }
}