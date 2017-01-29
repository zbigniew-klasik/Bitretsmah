using System;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Bitretsmah.Data.LiteDB;
using Bitretsmah.Data.LiteDB.Internal;
using Bitretsmah.Tests.Unit;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Integration.Data.LiteDB
{
    [TestFixture]
    public class BackupRepositoryShould
    {
        private IBackupRepository _backupRepository;

        [SetUp]
        public void SetUp()
        {
            var b1 = new Backup { TargetName = "Target One" };
            var b2 = new Backup { TargetName = "My Target" };
            var b3 = new Backup { TargetName = "Target Two" };
            var b4 = new Backup { TargetName = "My Target" };

            using (var db = DbFactory.Create())
            {
                db.DropCollection(Db.BackupsCollectionName);
                db.Backups.Insert(b1);
                db.Backups.Insert(b2);
                db.Backups.Insert(b3);
                db.Backups.Insert(b4);
            }

            _backupRepository = new BackupRepository();
        }

        [Test]
        public async Task GetAllForTarget()
        {
            var backups = await _backupRepository.GetAllForTarget("My Target");
            backups.Count.Should().Be(2);
            backups[0].TargetName.Should().Be("My Target");
            backups[1].TargetName.Should().Be("My Target");
        }

        [Test]
        public async Task AddNewBackup()
        {
            var newBackup = new Backup { TargetName = "My New Target", CreationTime = DateTimeOffset.Now };
            await _backupRepository.Add(newBackup);

            using (var db = DbFactory.Create())
            {
                var backup = db.Backups.FindOne(x => x.TargetName.Equals(newBackup.TargetName));
                backup.Should().NotBeNull();
                backup.TargetName.Should().Be(newBackup.TargetName);
            }
        }
    }
}