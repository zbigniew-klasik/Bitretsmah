using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bitretsmah.Core;
using NUnit.Framework;
using StructureMap;
using Bitretsmah.Core.Models;
using FluentAssertions;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Tests.Integration.Core
{
    public class BackupServiceShould
    {
        private IAccountService _accountService;
        private IBackupRepository _backupRepository;
        private IBackupService _backupService;
        private IRemoteFileWarehouseFactory _remoteFileWarehouseFactory;
        private ITargetService _targetService;

        [SetUp]
        public void Setup()
        {
            var registry = new Registry();
            registry.IncludeRegistry<Bitretsmah.Data.System.ContainerRegistry>();
            registry.IncludeRegistry<Bitretsmah.Data.LiteDB.ContainerRegistry>();
            registry.IncludeRegistry<Bitretsmah.Data.Mega.ContainerRegistry>();
            registry.IncludeRegistry<Bitretsmah.Core.ContainerRegistry>();
            registry.IncludeRegistry<Bitretsmah.UI.ConsoleApp.ContainerRegistry>(); // TODO: that should not be needed
            var container = new Container(registry);

            _accountService = container.GetInstance<IAccountService>();
            _backupService = container.GetInstance<IBackupService>();
            _backupRepository = container.GetInstance<IBackupRepository>();
            _remoteFileWarehouseFactory = container.GetInstance<IRemoteFileWarehouseFactory>();
            _targetService = container.GetInstance<ITargetService>();

            TestCleanUpHelper.CleanUpDatabase();
            TestCleanUpHelper.CleanUpMegaAccount(AppConfigHelper.GetTestMegaCredential());
        }

        [Test]
        public async Task CreateProperBackup()
        {
            var targetDirectory = new DirectoryInfo("Target Directory " + Guid.NewGuid());
            targetDirectory.Create();
            var filePath = Path.Combine(targetDirectory.FullName, "foobar.txt");
            System.IO.File.WriteAllText(filePath, "ABC");

            Console.WriteLine("Setting account...");
            await _accountService.SetCredential(AppConfigHelper.GetTestMegaCredential());

            Console.WriteLine("Setting target...");
            await _targetService.SetTarget("Test Target", targetDirectory.FullName);

            var backupRequest = new BackupRequest
            {
                TargetName = "Test Target",
                ComputeHashForEachFile = true,
                Progress = new Progress<BackupProgress>()
            };

            Console.WriteLine("Creating backup...");
            await _backupService.Backup(backupRequest);

            Console.WriteLine("Verifying remote file...");
            using (var remoteFileWarehouse = await _remoteFileWarehouseFactory.Create())
            {
                var remoteFilesList = await remoteFileWarehouse.GetFilesList();
                remoteFilesList.Count().Should().Be(1);
                var remoteFile = remoteFilesList.Single();
                remoteFile.Id.StoreId.Should().Be(AppConfigHelper.GetTestMegaCredential().UserName);
                remoteFile.Name.Should().Be("[3C01BDBB26F358BAB27F267924AA2C9A03FCFDB8]_foobar.txt");
                remoteFile.Size.Should().Be(3);
            }

            Console.WriteLine("Verifying local database...");
            var allBackups = await _backupRepository.GetAllForTarget("Test Target");
            allBackups.Count.Should().Be(1);
            var backup = allBackups.Single();
            backup.TargetName.Should().Be("Test Target");
            var changedFiles = backup.StructureChange.StructureToList();
            changedFiles.Count.Should().Be(2);
            changedFiles.First().Name.Should().Be(targetDirectory.Name);
            changedFiles.Last().Name.Should().Be("foobar.txt");

            // Console.WriteLine("Verifying remote data record...");
            // TODO: Verify remote data record

            targetDirectory.Delete(true);
            Console.WriteLine("OK!");
        }

        [Test]
        public async Task CreateBackupAndRestore()
        {
            Console.WriteLine("Creating target directory...");
            var targetDirectory = new DirectoryInfo("Target Directory " + Guid.NewGuid());
            targetDirectory.Create();

            Console.WriteLine("Creating file...");
            var filePath = Path.Combine(targetDirectory.FullName, "foobar.txt");
            System.IO.File.WriteAllText(filePath, "ABCDEF");

            Console.WriteLine("Setting account...");
            await _accountService.SetCredential(AppConfigHelper.GetTestMegaCredential());

            Console.WriteLine("Setting target...");
            await _targetService.SetTarget("Test Target", targetDirectory.FullName);

            Console.WriteLine("Creating backup...");
            var backupRequest = new BackupRequest
            {
                TargetName = "Test Target",
                ComputeHashForEachFile = true,
                Progress = new Progress<BackupProgress>()
            };
            await _backupService.Backup(backupRequest);

            Console.WriteLine("Deleting file...");
            System.IO.File.Delete(filePath);
            Assert.IsFalse(System.IO.File.Exists(filePath));

            Console.WriteLine("Restoring backup...");
            var restoreRequest = new RestoreRequest()
            {
                TargetName = "Test Target",
                ComputeHashForEachFile = true,
                Progress = new Progress<BackupProgress>()
            };
            await _backupService.Restore(restoreRequest);

            Console.WriteLine("Verifying file...");
            Assert.IsTrue(System.IO.File.Exists(filePath));
            Assert.AreEqual("ABCDEF", System.IO.File.ReadAllText(filePath));

            targetDirectory.Delete(true);
            Console.WriteLine("OK!");
        }

        [TearDown]
        public void TearDown()
        {
            TestCleanUpHelper.CleanUpDatabase();
            TestCleanUpHelper.CleanUpMegaAccount(AppConfigHelper.GetTestMegaCredential());
        }
    }
}