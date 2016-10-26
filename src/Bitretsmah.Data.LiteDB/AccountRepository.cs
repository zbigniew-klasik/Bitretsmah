using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Data.LiteDB
{
    public class AccountRepository : IAccountRepository
    {
        public Task<List<Account>> GetAll()
        {
            return Task.Run(() =>
            {
                using (var db = new LiteDatabase(@"bitretsmah.db"))
                {
                    var accounts = db.GetCollection<Account>("accounts");
                    return accounts.FindAll().ToList();
                }
            });
        }

        public Task<Account> GetByEmail(string email)
        {
            return Task.Run(() =>
            {
                using (var db = new LiteDatabase(@"bitretsmah.db"))
                {
                    var accounts = db.GetCollection<Account>("accounts");
                    return accounts.FindOne(x => x.Credential.UserName.Equals(email));
                }
            });
        }

        public Task AddAccount(Account account)
        {
            return Task.Run(() =>
            {
                using (var db = new LiteDatabase(@"bitretsmah.db"))
                {
                    var accounts = db.GetCollection<Account>("accounts");
                    accounts.Insert(account);
                }
            });
        }
    }
}