using Bitretsmah.Data.Mega;
using CG.Web.MegaApiClient;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using File = System.IO.File;

namespace Bitretsmah.Tests.Integration.Data.Mega
{
    [TestFixture]
    public class MegaStoreShould
    {
        [SetUp]
        public void SetUp()
        {
            CleanUpMegaAccount(AppConfigHelper.GetTestMegaCredential());
        }

        [Test]
        public async Task SimpleUploadDownload()
        {
            var fileName1 = Guid.NewGuid() + ".txt";
            var fileName2 = Guid.NewGuid() + ".txt";
            var fileContent = Guid.NewGuid().ToString();

            Console.WriteLine($"File name: {fileName1}");
            Console.WriteLine($"File content: {fileContent}");

            try
            {
                File.WriteAllText(fileName1, fileContent);

                var store = new MegaStore(AppConfigHelper.GetTestMegaCredential());

                var id = await store.UploadFile(fileName1, new Progress<double>());
                await store.DownloadFile(id, fileName2, new Progress<double>());

                var downloadedContent = File.ReadAllText(fileName2);
                downloadedContent.Should().Be(fileContent);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
            finally
            {
                Console.WriteLine("Finally:");
                File.Delete(fileName1);
                File.Delete(fileName2);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            CleanUpMegaAccount(AppConfigHelper.GetTestMegaCredential());
        }

        public void CleanUpMegaAccount(NetworkCredential credential)
        {
            var megaApiClient = new MegaApiClient();
            megaApiClient.Login(credential.UserName, credential.Password);
            var nodes = megaApiClient.GetNodes().ToList();
            nodes.Where(n => n.Type == NodeType.File).ToList().ForEach(n => megaApiClient.Delete(n, false));
            nodes.Where(n => n.Type == NodeType.Directory).ToList().ForEach(n => megaApiClient.Delete(n, false));
            megaApiClient.Logout();
        }
    }
}