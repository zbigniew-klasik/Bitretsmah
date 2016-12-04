using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class HistoryServiceShould
    {
        [Test]
        public void GetLastStructure()
        {
            var target = "Test Target";
            var backups = new List<Backup>();

            var changes = new List<Node>();

            var lastStructure = new Directory();

            var backupRepositoryMock = new Mock<IBackupRepository>();
            backupRepositoryMock.Setup(x => x.GetAllForTarget(target)).ReturnsAsync(backups);

            var nodeChangesApplier = new Mock<INodeChangesApplier>();
            //nodeChangesApplier.Setup(x => x.Apply())
        }
    }
}