using Bitretsmah.Core.Models;
using LiteDB;

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

        public LiteCollection<Account> Accounts => GetCollection<Account>("accounts");

        public static void Configure()
        {
            using (var db = new Db())
            {
                db.Accounts.EnsureIndex(x => x.Credential.UserName, true);
            }
        }
    }
}