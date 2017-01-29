using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class HistoryServiceShould
    {
        [Test]
        public async Task GetLastStructureAndCallsAllServices()
        {
            var target = "Test Target";
            var backups = new List<Backup>()
            {
                new Backup() { CreationTime = DateTimeOffset.Now.AddDays(-3), StructureChange = new Directory { Name = "Temp" }, TargetName = target },
                new Backup() { CreationTime = DateTimeOffset.Now.AddDays(-2), StructureChange = new Directory { Name = "Temp" }, TargetName = target },
                new Backup() { CreationTime = DateTimeOffset.Now.AddDays(-1), StructureChange = new Directory { Name = "Temp" }, TargetName = target },
            };

            var expectedLastStructure = new Directory { Name = "Temp" };

            var backupRepositoryMock = new Mock<IBackupRepository>();
            backupRepositoryMock.Setup(x => x.GetAllForTarget(target)).ReturnsAsync(backups);

            var nodeChangesApplier = new Mock<INodeChangesApplier>();
            nodeChangesApplier.Setup(x => x.Apply(It.IsAny<ICollection<Node>>())).Returns(expectedLastStructure);

            var historyService = new HistoryService(backupRepositoryMock.Object, nodeChangesApplier.Object);
            var actualLastStructure = await historyService.GetLastStructure(target);

            backupRepositoryMock.Verify(x => x.GetAllForTarget(target), Times.Once());
            nodeChangesApplier.Verify(x => x.Apply(It.IsAny<ICollection<Node>>()), Times.Once);
            actualLastStructure.ShouldBeEquivalentTo(expectedLastStructure);
            actualLastStructure.ShouldSerializeSameAs(expectedLastStructure);
        }

        [Test]
        public async Task GetLastStructureFromBackupsInCorrectOrder()
        {
            var target = "Test Target";
            var backups = new List<Backup>()
            {
                new Backup() { CreationTime = DateTimeOffset.Now.AddMinutes(-1), StructureChange = new Directory { Name = "F" }, TargetName = target },
                new Backup() { CreationTime = DateTimeOffset.Now.AddDays(-4), StructureChange = new Directory { Name = "A" }, TargetName = target },
                new Backup() { CreationTime = DateTimeOffset.Now.AddDays(-2), StructureChange = new Directory { Name = "C" }, TargetName = target },
                new Backup() { CreationTime = DateTimeOffset.Now.AddHours(-1), StructureChange = new Directory { Name = "E" }, TargetName = target },
                new Backup() { CreationTime = DateTimeOffset.Now.AddDays(-1), StructureChange = new Directory { Name = "D" }, TargetName = target },
                new Backup() { CreationTime = DateTimeOffset.Now.AddDays(-3), StructureChange = new Directory { Name = "B" }, TargetName = target },
            };

            var lastStructure = new Directory { Name = "Temp" };

            var backupRepositoryMock = new Mock<IBackupRepository>();
            backupRepositoryMock.Setup(x => x.GetAllForTarget(target)).ReturnsAsync(backups);

            ICollection<Node> actualNodes = null;
            var nodeChangesApplier = new Mock<INodeChangesApplier>();
            nodeChangesApplier.Setup(x => x.Apply(It.IsAny<ICollection<Node>>()))
                .Callback<ICollection<Node>>(x => actualNodes = x)
                .Returns(lastStructure);

            var historyService = new HistoryService(backupRepositoryMock.Object, nodeChangesApplier.Object);
            await historyService.GetLastStructure(target);

            var expectedOrder = new string[] { "A", "B", "C", "D", "E", "F" };
            var actualOrder = actualNodes.Select(x => x.Name).ToArray();

            actualOrder.ShouldAllBeEquivalentTo(expectedOrder);
            actualOrder.ShouldSerializeSameAs(expectedOrder);
        }

        [Test]
        public async Task GetLastStructureReturnsNullForEmptyBackups()
        {
            var target = "Test Target";
            List<Backup> backups = new List<Backup>();
            var lastStructure = new Directory { Name = "Temp" };

            var backupRepositoryMock = new Mock<IBackupRepository>();
            backupRepositoryMock.Setup(x => x.GetAllForTarget(target)).ReturnsAsync(backups);

            var nodeChangesApplier = new Mock<INodeChangesApplier>();
            nodeChangesApplier.Setup(x => x.Apply(It.IsAny<ICollection<Node>>())).Returns(lastStructure);

            var historyService = new HistoryService(backupRepositoryMock.Object, nodeChangesApplier.Object);
            var result = await historyService.GetLastStructure(target);

            result.Should().Be(null);
        }

        [Test]
        public async Task GetLastStructureReturnsNullForNullBackups()
        {
            var target = "Test Target";
            List<Backup> backups = null;
            var lastStructure = new Directory { Name = "Temp" };

            var backupRepositoryMock = new Mock<IBackupRepository>();
            backupRepositoryMock.Setup(x => x.GetAllForTarget(target)).ReturnsAsync(backups);

            var nodeChangesApplier = new Mock<INodeChangesApplier>();
            nodeChangesApplier.Setup(x => x.Apply(It.IsAny<ICollection<Node>>())).Returns(lastStructure);

            var historyService = new HistoryService(backupRepositoryMock.Object, nodeChangesApplier.Object);
            var result = await historyService.GetLastStructure(target);

            result.Should().Be(null);
        }
    }
}