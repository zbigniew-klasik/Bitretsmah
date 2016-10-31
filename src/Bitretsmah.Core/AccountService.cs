using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public interface IAccountService
    {
        Task SetCredential(NetworkCredential credential);

        Task<IEnumerable<Account>> GetAll();
    }

    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task SetCredential(NetworkCredential credential)
        {
            await _accountRepository.AddAccount(new Account { Credential = credential });
            // toto: or update
            // todo: test if this account works
        }

        public async Task<IEnumerable<Account>> GetAll()
        {
            return await _accountRepository.GetAll();
        }
    }
}