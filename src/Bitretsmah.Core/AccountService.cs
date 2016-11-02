﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bitretsmah.Core.Exceptions;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;

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
        private readonly ICredentialVerifier _credentialVerifier;

        public AccountService(IAccountRepository accountRepository, ICredentialVerifier credentialVerifier)
        {
            _accountRepository = accountRepository;
            _credentialVerifier = credentialVerifier;
        }

        public async Task<IEnumerable<Account>> GetAll()
        {
            return await _accountRepository.GetAll();
        }

        public async Task SetCredential(NetworkCredential credential)
        {
            if (await _credentialVerifier.Verify(credential))
            {
                await _accountRepository.AddOrUpdate(credential);
            }
            else
            {
                throw new InvalidCredentialException();
            }
        }
    }
}