using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAll();

        Task<Account> GetByEmail(string email);

        Task AddAccount(Account account);
    }
}