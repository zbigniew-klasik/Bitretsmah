using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitretsmah.Data.LiteDB.Internal;

namespace Bitretsmah.Data.LiteDB
{
    public class AccountRepository : IAccountRepository
    {
        public Task<List<Account>> GetAll()
        {
            return Task.Run(() =>
            {
                using (var db = DbFactory.Create())
                {
                    return db.Accounts.FindAll().ToList();
                }
            });
        }

        public Task<Account> GetByEmail(string email)
        {
            return Task.Run(() =>
            {
                using (var db = DbFactory.Create())
                {
                    return db.Accounts.FindOne(x => x.Credential.UserName.Equals(email));
                }
            });
        }

        public Task AddAccount(Account account)
        {
            return Task.Run(() =>
            {
                using (var db = DbFactory.Create())
                {
                    return db.Accounts.Insert(account);
                }
            });
        }
    }
}