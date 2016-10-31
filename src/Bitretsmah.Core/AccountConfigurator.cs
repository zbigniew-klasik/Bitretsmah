using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public class AccountConfigurator
    {
        private IAccountRepository _accountRepository;

        public AccountConfigurator(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task SetAccount(NetworkCredential credential)
        {
            await _accountRepository.AddAccount(new Account { Credential = credential });
            // toto: or update
            // todo: test if this account works
        }

        public async Task<IEnumerable<Account>> GetAllAccounts()
        {
            return await _accountRepository.GetAll();
        }
    }
}