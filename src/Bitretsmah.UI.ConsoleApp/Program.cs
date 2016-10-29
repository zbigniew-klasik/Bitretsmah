using Fclp;
using System;

namespace Bitretsmah.UI.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var parser = new FluentCommandLineParser<ApplicationArguments>();

            parser.SetupHelp("h", "help").Callback(text => Console.WriteLine(text)).UseForEmptyArgs();
            parser.Setup(x => x.ShowVersion).As('v', "version").WithDescription("Shows the version of that application.");
            parser.Setup(x => x.AddAccount).As("add-account").WithDescription("Adds a storage account.");
            parser.Setup(x => x.AddUpdate).As("update-account").WithDescription("Updates a storrage account.");
            parser.Setup(x => x.Silent).As('s', "silent").WithDescription("Runs with the silent mode.");
            parser.Setup(x => x.Force).As('F', "froced").WithDescription("Runs with the forced mode."); ;

            var result = parser.Parse(args);

            if (result.HasErrors == false)
            {
                if (parser.Object.ShowVersion) ShowVersion();
                if (parser.Object.AddAccount) AddAccount();
                if (parser.Object.AddUpdate) UpdateAccount();
            }
            else
            {
                Console.WriteLine(result.ErrorText);
            }

            foreach (var keyValuePair in result.AdditionalOptionsFound)
            {
                Console.WriteLine($"AdditionalOptionsFound: key: {keyValuePair.Key} value: {keyValuePair.Value}");
            }

            Console.WriteLine("Bitretsmah finished!");
        }

        private static void ShowVersion()
        {
            Console.WriteLine("ShowVersion");
        }

        private static void AddAccount()
        {
            Console.WriteLine("AddAccount");
        }

        private static void UpdateAccount()
        {
            Console.WriteLine("UpdateAccount");
        }
    }

    public class ApplicationArguments
    {
        public bool ShowVersion { get; set; }
        public bool AddAccount { get; set; }
        public bool AddUpdate { get; set; }

        public bool Silent { get; set; }
        public bool Force { get; set; }
    }
}