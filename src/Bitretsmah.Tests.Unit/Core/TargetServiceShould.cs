using System;
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
    public class TargetServiceShould
    {
        private Mock<ILocalFilesService> _localFilesService;
        private Mock<ITargetRepository> _targetRepositoryMock;
        private ITargetService _targetService;

        [SetUp]
        public void SetUp()
        {
            _localFilesService = new Mock<ILocalFilesService>();
            _targetRepositoryMock = new Mock<ITargetRepository>();
            _targetService = new TargetService(_localFilesService.Object, _targetRepositoryMock.Object);
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
        public async Task GetByName()
        {
            var name = "My Target";
            var target = new Target { Name = name };

            _targetRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(target);

            var result = await _targetService.GetByName(name);

            result.Should().Be(target);
            _targetRepositoryMock.Verify(x => x.GetByName(name), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetByName_ForEmptyName_ThrowsException(string name)
        {
            Assert.That(() => _targetService.GetByName(name), Throws.TypeOf<EmptyTargetNameException>());
            _targetRepositoryMock.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetByName_ForUnknownName_ThrowsException()
        {
            var name = "My Target";
            _targetRepositoryMock.Setup(x => x.GetByName(name)).Returns(Task.FromResult<Target>(null));

            Assert.That(() => _targetService.GetByName(name), Throws.TypeOf<UnknownTargetException>());
            _targetRepositoryMock.Verify(x => x.GetByName(name), Times.Once);
        }

        [Test]
        public async Task SetTarget()
        {
            var name = "My Target";
            var path = @"B:\targets\my_target";

            _localFilesService.Setup(x => x.Exists(path)).Returns(true);

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

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void SetTarget_WithEmptyName_ThrowsException(string name)
        {
            var path = @"C:\targets\my_target";
            Assert.That(() => _targetService.SetTarget(name, path), Throws.TypeOf<EmptyTargetNameException>());
            _targetRepositoryMock.Verify(x => x.AddOrUpdate(It.IsAny<Target>()), Times.Never);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void SetTarget_WithEmptyPath_ThrowsException(string path)
        {
            var name = @"My Target";
            Assert.That(() => _targetService.SetTarget(name, path), Throws.TypeOf<EmptyTargetPathException>());
            _targetRepositoryMock.Verify(x => x.AddOrUpdate(It.IsAny<Target>()), Times.Never);
        }

        [Test]
        public void SetTarget_WithInvalidPath_ThrowsException()
        {
            var name = "My Target";
            var path = @"B:\targets\my_target";

            _localFilesService.Setup(x => x.Exists(path)).Returns(false);

            Assert.That(() => _targetService.SetTarget(name, path), Throws.TypeOf<InvalidTargetPathException>());
            _targetRepositoryMock.Verify(x => x.AddOrUpdate(It.IsAny<Target>()), Times.Never);
        }
    }
}