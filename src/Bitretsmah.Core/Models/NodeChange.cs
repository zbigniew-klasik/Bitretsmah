using System.Collections.Generic;

namespace Bitretsmah.Core.Models
{
    public class NodeChange
    {
        public ChangeType Type { get; set; }
        public Node Node { get; set; }
        public List<NodeChange> InnerChanges { get; set; }
    }
}