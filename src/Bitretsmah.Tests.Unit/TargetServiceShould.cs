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

namespace Bitretsmah.Tests.Unit
{
    [TestFixture]
    public class TargetServiceShould
    {
        private Mock<ITargetRepository> _targetRepositoryMock;
        private ITargetService _targetService;

        [SetUp]
        public void SetUp()
        {
            _targetRepositoryMock = new Mock<ITargetRepository>();
            _targetService = new TargetService(_targetRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllTargets()
        {
            var targets = new List<Target>() { new Target { Name = "A" }, new Target { Name = "B" } };
            _targetRepositoryMock.Setup(x => x.GetAll()).ReturnsAsync(targets);

            var result = await _targetService.GetAll();

            _targetRepositoryMock.Verify(x => x.GetAll(), Times.Once());
            result.ShouldBeEquivalentTo(targets);
            result.ShouldSerializeSameAs(targets);
        }

        [Test]
        public async Task SetTarget()
        {
            var name = "My Target";
            var path = @"B:\targets\my_target";

            Target actualTarget = null;
            _targetRepositoryMock
                .Setup(x => x.AddOrUpdate(It.IsAny<Target>()))
                .Returns(Task.CompletedTask)
                .Callback<Target>(x => actualTarget = x);

            await _targetService.SetTarget(name, path);

            _targetRepositoryMock.Verify(x => x.AddOrUpdate(It.IsAny<Target>()), Times.Once);
            actualTarget.Should().NotBeNull();
            actualTarget.Name.Should().Be(name);
            actualTarget.LocalPath.Should().Be(path);
        }
    }
}