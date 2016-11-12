using Bitretsmah.Core.Models;
using LiteDB;
using System.Net;

namespace Bitretsmah.Data.LiteDB.Internal
{
    internal class Db : LiteDatabase
    {
        private const string ConnecionString = @"bitretsmah.db";

        static Db()
        {
            Configure();
        }

        public Db()
            : base(ConnecionString)
        {
        }

        public const string AccountsCollectionName = "accounts";

        public LiteCollection<Account> Accounts => GetCollection<Account>(AccountsCollectionName);

        private static void Configure()
        {
            BsonMapper.Global.Entity<NetworkCredential>()
                .Index(x => x.UserName, true)
                .Field(x => x.Password, "Password")
                .Ignore(x => x.SecurePassword)
                .Ignore(x => x.Domain);
        }
    }
}