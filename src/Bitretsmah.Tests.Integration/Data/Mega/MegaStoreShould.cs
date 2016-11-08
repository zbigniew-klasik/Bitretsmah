using CG.Web.MegaApiClient;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;

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
        public void Test()
        {
            Console.WriteLine("test");
        }

        [OneTimeTearDown]
        public void TearDown()
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