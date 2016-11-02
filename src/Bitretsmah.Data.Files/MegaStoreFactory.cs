using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Data.Mega
{
    public class MegaStoreFactory : IRemoteFileStoreFactory
    {
        private readonly IAccountService _accountService;

        public MegaStoreFactory(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IList<IRemoteFileStore>> GetAll()
        {
            var accounts = await _accountService.GetAll();
            return accounts.Select(x => new MegaStore(x.Credential) as IRemoteFileStore).ToList();
        }
    }
}