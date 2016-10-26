using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
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

        public async Task AddMegaAccount(NetworkCredential credential)
        {
            await _accountRepository.AddAccount(new Account { Credential = credential });
            // todo: test if this account works
        }
    }
}