using System;
using System.Collections.Generic;

namespace Bitretsmah.Core.Models
{
    [Serializable]
    public class Directory : Node
    {
        public Directory()
        {
            InnerNodes = new List<Node>();
        }

        public List<Node> InnerNodes { get; set; }
    }
}