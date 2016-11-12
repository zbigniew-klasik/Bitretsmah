﻿using Bitretsmah.Data.Mega;
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
            CleanUpMegaAccount(AppConfigHelper.GetTestMegaCredential());
        }

        [Test]
        public async Task UploadFileAndDownloadFile()
        {
            var fileName1 = Guid.NewGuid() + ".txt";
            var fileName2 = Guid.NewGuid() + ".txt";
            var fileContent = Guid.NewGuid().ToString();

            Console.WriteLine("Creating store...");
            var store = new MegaStore(AppConfigHelper.GetTestMegaCredential());

            Console.WriteLine("Verifying quota before upload...");
            var quotaBeforeUpload = await store.GetQuota();
            quotaBeforeUpload.Total.Should().Be(Quota50GB);
            quotaBeforeUpload.Free.Should().Be(Quota50GB);

            Console.WriteLine("Writing file...");
            File.WriteAllText(fileName1, fileContent);

            Console.WriteLine("Uploading file...");
            var id = await store.UploadFile(fileName1, new Progress<double>());

            Console.WriteLine("Verifying quota after upload...");
            var quotaAfterUpload = await store.GetQuota();
            quotaAfterUpload.Total.Should().Be(Quota50GB);
            quotaAfterUpload.Free.Should().Be(Quota50GB - new FileInfo(fileName1).Length);

            Console.WriteLine("Downloading file...");
            await store.DownloadFile(id, fileName2, new Progress<double>());

            Console.WriteLine("Reading file...");
            var downloadedContent = File.ReadAllText(fileName2);
            downloadedContent.Should().Be(fileContent);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            CleanUpMegaAccount(AppConfigHelper.GetTestMegaCredential());
        }

        public void CleanUpMegaAccount(NetworkCredential credential)
        {
            Console.WriteLine("Cleaning...");
            var megaApiClient = new MegaApiClient();
            megaApiClient.Login(credential.UserName, credential.Password);
            var nodes = megaApiClient.GetNodes().ToList();
            nodes.Where(n => n.Type == NodeType.File).ToList().ForEach(n => megaApiClient.Delete(n, false));
            nodes.Where(n => n.Type == NodeType.Directory).ToList().ForEach(n => megaApiClient.Delete(n, false));
            megaApiClient.Logout();
        }
    }
}