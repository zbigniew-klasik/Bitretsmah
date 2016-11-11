using System.Collections.Generic;

namespace Bitretsmah.Core.Models
{
    public class Directory : Node
    {
        public string Name { get; set; }
        public List<Node> Nodes { get; set; }
    }
}