using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.UI.ConsoleApp
{
    internal interface IExecutor
    {
        Task Execut(ConsoleArguments arguments);
    }

    internal class Executor : IExecutor
    {
        private readonly IConsoleArgumentsParser _consoleArgumentsParser;
        private readonly IConsoleService _consoleService;
        private readonly ITargetRepository _targetRepository;

        public Executor(IConsoleArgumentsParser consoleArgumentsParser, IConsoleService consoleService, ITargetRepository targetRepository)
        {
            _consoleArgumentsParser = consoleArgumentsParser;
            _consoleService = consoleService;
            _targetRepository = targetRepository;
        }

        public async Task Execut(ConsoleArguments arguments)
        {
            await Task.FromResult(0);
        }
    }
}