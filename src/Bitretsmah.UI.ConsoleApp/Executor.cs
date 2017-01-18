using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Executor(IConsoleArgumentsParser consoleArgumentsParser, IConsoleService consoleService)
        {
            _consoleArgumentsParser = consoleArgumentsParser;
            _consoleService = consoleService;
        }

        public async Task Execut(ConsoleArguments arguments)
        {
            await Task.FromResult(0);
        }
    }
}