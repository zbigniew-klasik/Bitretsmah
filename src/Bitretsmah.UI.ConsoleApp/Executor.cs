using Bitretsmah.Core;
using Bitretsmah.Core.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.UI.ConsoleApp
{
    internal interface IExecutor
    {
        Task Execut(ConsoleArguments arguments);
    }

    internal class Executor : IExecutor
    {
        private readonly IAccountService _accountService;
        private readonly IConsoleService _consoleService;
        private readonly ITargetService _targetService;

        public Executor(IAccountService accountService, IConsoleService consoleService, ITargetService targetService)
        {
            _accountService = accountService;
            _consoleService = consoleService;
            _targetService = targetService;
        }

        public async Task Execut(ConsoleArguments arguments)
        {
            try
            {
                if (arguments.SetAccount != null)
                    await SetAccount(arguments.SetAccount, arguments.Password);

                if (arguments.Accounts)
                    await ListAllAccounts();

                if (arguments.SetTarget != null)
                    await SetTarget(arguments.SetTarget, arguments.Path);

                if (arguments.Targets)
                    await ListAllTargets();
            }
            catch (BitretsmahException ex)
            {
                //TODO: log
                _consoleService.WriteErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                //TODO: log
                _consoleService.WriteUnexpectedException(ex);
            }
        }

        private async Task SetAccount(string username, string password)
        {
            var credential = string.IsNullOrWhiteSpace(password)
                ? new NetworkCredential(username, _consoleService.ReadPassword())
                : new NetworkCredential(username, password);

            await _accountService.SetCredential(credential);
            _consoleService.AccountSetSuccessfully();
        }

        private async Task ListAllAccounts()
        {
            var accounts = await _accountService.GetAll();
            _consoleService.ListAccounts(accounts);
        }

        private async Task SetTarget(string name, string path)
        {
            await _targetService.SetTarget(name, path);
            _consoleService.TargetSetSuccessfully();
        }

        private async Task ListAllTargets()
        {
            var targets = await _targetService.GetAll();
            _consoleService.ListTargets(targets);
        }
    }
}