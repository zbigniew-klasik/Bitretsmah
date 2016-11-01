using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Bitretsmah.Data.Mega
{
    public class MegaStoreFactory
    {
        private readonly IAccountService _accountService;

        public MegaStoreFactory(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IList<IRemoteFileStore> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}