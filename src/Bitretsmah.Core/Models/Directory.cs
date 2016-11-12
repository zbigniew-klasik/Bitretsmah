using System.Collections.Generic;

namespace Bitretsmah.Core.Models
{
    public class Directory : Node
    {
        public Directory()
        {
            InnerNodes = new List<Node>();
        }

        public Directory(string name)
            : this()
        {
            Name = name;
        }

        public List<Node> InnerNodes { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}