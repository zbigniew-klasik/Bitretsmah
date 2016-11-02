using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
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

        public Task AddOrUpdate(NetworkCredential credential)
        {
            return Task.Run(() =>
            {
                using (var db = DbFactory.Create())
                {
                    var existingAccount = db.Accounts.FindOne(x => x.Credential.UserName.Equals(credential.UserName));

                    if (existingAccount == null)
                    {
                        db.Accounts.Insert(new Account { Credential = credential });
                    }
                    else
                    {
                        existingAccount.Credential = credential;
                        db.Accounts.Update(existingAccount);
                    }
                }
            });
        }
    }
}