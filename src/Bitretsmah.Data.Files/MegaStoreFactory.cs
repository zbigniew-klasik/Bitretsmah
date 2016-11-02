using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;

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
            throw new NotImplementedException();
        }
    }
}