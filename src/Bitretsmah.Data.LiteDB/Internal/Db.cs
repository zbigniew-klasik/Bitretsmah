using Bitretsmah.Core.Models;
using LiteDB;

namespace Bitretsmah.Data.LiteDB.Internal
{
    internal class Db : LiteDatabase
    {
        public Db(string connectionString)
            : base(connectionString)
        {
        }

        public LiteCollection<Account> Accounts => this.GetCollection<Account>("accounts");
    }
}