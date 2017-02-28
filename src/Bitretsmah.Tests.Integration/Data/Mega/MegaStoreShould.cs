using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Bitretsmah.Data.Mega;
using CG.Web.MegaApiClient;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using File = System.IO.File;

namespace Bitretsmah.Tests.Integration.Data.Mega
{
    [TestFixture]
    public class MegaStoreShould
    {
        private decimal Quota50GB = 53687091200M;

        [SetUp]
        public void SetUp()
        {
            TestCleanUpHelper.CleanUpMegaAccount(AppConfigHelper.GetTestMegaCredential());
        }

        [Test]
        public async Task UploadFileAndDownloadFile()
        {
            var fileName1 = Guid.NewGuid() + ".txt";
            var fileName2 = Guid.NewGuid() + ".txt";
            var remoteName = Guid.NewGuid() + ".txt";
            var fileContent = Guid.NewGuid().ToString();

            Console.WriteLine("Creating store...");
            IRemoteFileStore store = new MegaStore(AppConfigHelper.GetTestMegaCredential());

            Console.WriteLine("Verifying quota before upload...");
            var quotaBeforeUpload = await store.GetQuota();
            quotaBeforeUpload.Total.Should().Be(Quota50GB);
            quotaBeforeUpload.Free.Should().Be(Quota50GB);

            Console.WriteLine("Writing file...");
            File.WriteAllText(fileName1, fileContent);

            Console.WriteLine("Uploading file...");
            RemoteId remoteId = null;
            using (var fileStream = new FileStream(fileName1, FileMode.Open, FileAccess.Read))
            {
                remoteId = await store.UploadFile(fileStream, remoteName, new Progress<double>());
            }

            Console.WriteLine("Verifying uploaded file...");
            var remoteFiles = await store.GetFilesList();
            remoteFiles.Count.Should().Be(1);
            var remoteFile = remoteFiles.First();
            remoteFile.Id.StoreId.Should().Be(store.StoreId);
            remoteFile.Name.Should().Be(remoteName);
            remoteFile.Size.Should().Be(new FileInfo(fileName1).Length);

            Console.WriteLine("Verifying quota after upload...");
            var quotaAfterUpload = await store.GetQuota();
            quotaAfterUpload.Total.Should().Be(Quota50GB);
            quotaAfterUpload.Free.Should().Be(Quota50GB - new FileInfo(fileName1).Length);

            Console.WriteLine("Downloading file...");
            var remoteStream = await store.DownloadFile(remoteId, new Progress<double>());

            using (var fileStream = File.Create(fileName2))
            {
                await remoteStream.CopyToAsync(fileStream);
            }

            Console.WriteLine("Reading file...");
            var downloadedContent = File.ReadAllText(fileName2);
            downloadedContent.Should().Be(fileContent);

            Console.WriteLine("Deleting local files...");
            File.Delete(fileName1);
            File.Delete(fileName2);
        }

        [TearDown]
        public void TearDown()
        {
            TestCleanUpHelper.CleanUpMegaAccount(AppConfigHelper.GetTestMegaCredential());
        }
    }
}