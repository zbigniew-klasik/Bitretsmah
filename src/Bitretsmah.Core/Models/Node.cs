﻿using System;
using System.Collections.Generic;

namespace Bitretsmah.Core.Models
{
    [Serializable]
    public abstract class Node
    {
        public string Name { get; set; }
        public string AbsolutePath { get; set; }
        public NodeState State { get; set; }

        public override string ToString()
        {
            return State == NodeState.None ? Name : $"{Name} - {State}";
        }

        public virtual void SetAllStates(NodeState state)
        {
            State = state;
        }

        public virtual ICollection<Node> StructureToList()
        {
            return new List<Node>() { this };
        }
    }
}