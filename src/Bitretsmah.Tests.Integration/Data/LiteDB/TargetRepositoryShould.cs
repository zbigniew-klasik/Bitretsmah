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
    public class TargetRepositoryShould
    {
        private ITargetRepository _targetRepository;

        private Target _firstTarget;
        private Target _secondTarget;

        [SetUp]
        public void SetUp()
        {
            _firstTarget = new Target { Name = "My Target 1", LocalPath = @"C:\folder_1" };
            _secondTarget = new Target { Name = "My Target 2", LocalPath = @"C:\folder_2" };

            using (var db = DbFactory.Create())
            {
                db.DropCollection(Db.TargetsCollectionName);
                db.Targets.Insert(_firstTarget);
                db.Targets.Insert(_secondTarget);
            }

            _targetRepository = new TargetRepository();
        }

        [Test]
        public async Task GetAllTargets()
        {
            var targets = await _targetRepository.GetAll();
            targets.Count.Should().Be(2);
            targets[0].ShouldSerializeSameAs(_firstTarget);
            targets[1].ShouldSerializeSameAs(_secondTarget);
        }

        [Test]
        public async Task AddNewTarget()
        {
            var newTarget = new Target { Name = "My Target 3", LocalPath = @"C:\folder_3" };
            await _targetRepository.AddOrUpdate(newTarget);

            using (var db = DbFactory.Create())
            {
                var target = db.Targets.FindOne(x => x.Name.Equals(newTarget.Name));
                target.Should().NotBeNull();
                target.LocalPath.Should().Be(newTarget.LocalPath);
            }
        }

        [Test]
        public async Task UpdateExistingTarget()
        {
            var updatedTarget = new Target { Name = "My Target 2", LocalPath = @"E:\different_folder" };
            await _targetRepository.AddOrUpdate(updatedTarget);

            using (var db = DbFactory.Create())
            {
                var target = db.Targets.FindOne(x => x.Name.Equals(updatedTarget.Name));
                target.Should().NotBeNull();
                target.LocalPath.Should().Be(updatedTarget.LocalPath);
            }
        }
    }
}