using Bitretsmah.Core;
using Bitretsmah.Core.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;

namespace Bitretsmah.UI.ConsoleApp
{
    internal interface IExecutor
    {
        Task Execut(ConsoleArguments arguments);
    }

    internal class Executor : IExecutor
    {
        private readonly IAccountService _accountService;
        private readonly IBackupService _backupService;
        private readonly IConsoleService _consoleService;
        private readonly ILogger _logger;
        private readonly ITargetService _targetService;

        public Executor(IAccountService accountService, IBackupService backupService, IConsoleService consoleService, ILogger logger, ITargetService targetService)
        {
            _accountService = accountService;
            _backupService = backupService;
            _consoleService = consoleService;
            _logger = logger;
            _targetService = targetService;
        }

        public async Task Execut(ConsoleArguments arguments)
        {
            try
            {
                if (arguments.Help)
                {
                    _consoleService.WriteHelp();
                    return;
                }

                if (arguments.Version)
                {
                    _consoleService.WriteVersion();
                    return;
                }

                if (arguments.SetAccount != null)
                {
                    await SetAccount(arguments.SetAccount, arguments.Password);
                    return;
                }

                if (arguments.Accounts)
                {
                    await ListAllAccounts();
                    return;
                }

                if (arguments.SetTarget != null)
                {
                    await SetTarget(arguments.SetTarget, arguments.Path);
                    return;
                }

                if (arguments.RemoveTarget != null)
                {
                    await RemoveTarget(arguments.RemoveTarget);
                    return;
                }

                if (arguments.Targets)
                {
                    await ListAllTargets();
                    return;
                }

                if (arguments.Backup != null)
                {
                    await Backup(arguments.Backup); //TODO: unit test
                    return;
                }

                _consoleService.WriteHelp(); //TODO: unit test
            }
            catch (BitretsmahException ex)
            {
                _logger.Warn(ex);
                _consoleService.WriteErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _consoleService.WriteUnexpectedException(ex);
            }
        }

        private async Task Backup(string targetName)
        {
            //TODO: handle progress in proper way
            //TODO: use ComputeHashForEachFile (add command line flag)
            //TODO: unit test all of it

            var progress = new Progress<BackupProgress>();
            progress.ProgressChanged += (sender, backupProgress) => { _consoleService.WriteProgress(backupProgress); };

            var request = new BackupRequest
            {
                TargetName = targetName,
                ComputeHashForEachFile = false,
                Progress = progress
            };

            await _backupService.Backup(request);
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

        private async Task RemoveTarget(string name)
        {
            await _targetService.RemoveTarget(name);
            _consoleService.TargetRemovedSuccessfully();
        }

        private async Task ListAllTargets()
        {
            var targets = await _targetService.GetAll();
            _consoleService.ListTargets(targets);
        }
    }
}