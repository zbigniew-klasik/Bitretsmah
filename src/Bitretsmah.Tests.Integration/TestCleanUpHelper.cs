using Bitretsmah.Data.LiteDB.Internal;
using CG.Web.MegaApiClient;
using System;
using System.Linq;
using System.Net;

namespace Bitretsmah.Tests.Integration
{
    public static class TestCleanUpHelper
    {
        public static void CleanUpMegaAccount(NetworkCredential credential)
        {
            Console.WriteLine("Cleaning account: " + credential.UserName + "...");
            var megaApiClient = new MegaApiClient();
            megaApiClient.Login(credential.UserName, credential.Password);
            var nodes = megaApiClient.GetNodes().ToList();
            nodes.Where(n => n.Type == NodeType.File).ToList().ForEach(n => megaApiClient.Delete(n, false));
            nodes.Where(n => n.Type == NodeType.Directory).ToList().ForEach(n => megaApiClient.Delete(n, false));
            megaApiClient.Logout();
        }

        public static void CleanUpDatabase()
        {
            Console.WriteLine("Cleaning account database...");
            using (var db = DbFactory.Create())
            {
                db.DropCollection(Db.AccountsCollectionName);
                db.DropCollection(Db.TargetsCollectionName);
                db.DropCollection(Db.BackupsCollectionName);
            }
        }
    }
}