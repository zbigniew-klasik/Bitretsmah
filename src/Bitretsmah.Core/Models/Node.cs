using System;

namespace Bitretsmah.Core.Models
{
    [Serializable]
    public abstract class Node
    {
        public string Name { get; set; }
        public string AbsolutePath { get; set; }
        public NodeState State { get; set; }
    }
}