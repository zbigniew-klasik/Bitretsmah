using System;
using System.Threading.Tasks;
using Bitretsmah.Core;
using Bitretsmah.Core.Exceptions;

namespace Bitretsmah.UI.ConsoleApp
{
    internal interface IExecutor
    {
        Task Execut(ConsoleArguments arguments);
    }

    internal class Executor : IExecutor
    {
        private readonly IConsoleService _consoleService;
        private readonly ITargetService _targetService;

        public Executor(IConsoleService consoleService, ITargetService targetService)
        {
            _consoleService = consoleService;
            _targetService = targetService;
        }

        public async Task Execut(ConsoleArguments arguments)
        {
            try
            {
                if (arguments.SetTarget != null)
                    await SetTarget(arguments.SetTarget, arguments.Path);

                if (arguments.Targets)
                    await ListAllTargets();
            }
            catch (BitretsmahException exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async Task SetTarget(string name, string path)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidTargetName(name);
            }

            if (string.IsNullOrWhiteSpace(path)) // TODO: also check if path exists
            {
                throw new InvalidTargetPath(path);
            }

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