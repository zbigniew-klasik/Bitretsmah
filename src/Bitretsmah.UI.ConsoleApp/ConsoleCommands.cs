using System.Collections.Generic;

namespace Bitretsmah.UI.ConsoleApp
{
    public class ConsoleCommands
    {
        public bool ShowVersion { get; set; }
        public List<string> SetAccount { get; set; }
        public List<string> SetDirectory { get; set; }
        public bool Silent { get; set; }
        public bool Force { get; set; }
    }
}