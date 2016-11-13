using System.Collections.Generic;

namespace Bitretsmah.Core.Models
{
    public class NodeChange
    {
        public NodeChange()
        {
            InnerChanges = new List<NodeChange>();
        }

        public NodeChange(NodeState type, Node node)
            : this()
        {
            Type = type;
            Node = node;
        }

        public NodeState Type { get; set; }
        public Node Node { get; set; }
        public List<NodeChange> InnerChanges { get; set; }

        public override string ToString()
        {
            return $"{Type} {Node.Name}";
        }
    }
}