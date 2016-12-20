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

        public override void SetAllStates(NodeState state)
        {
            base.SetAllStates(state);
            InnerNodes.ForEach(x => x.SetAllStates(state));
        }

        public override ICollection<Node> StructureToList()
        {
            var list = new List<Node>() { this };
            InnerNodes.ForEach(x => list.AddRange(x.StructureToList()));
            return list;
        }
    }
}