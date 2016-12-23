using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;

namespace Bitretsmah.Tests.Unit.Core
{
    public static class NodesTestHelper
    {
        public static File CreateFile(string name, NodeState state, string hash = "hash")
        {
            return new File
            {
                Name = name,
                State = state,
                Hash = hash,
                Size = 1234,
                CreationTime = new DateTimeOffset(2016, 10, 11, 18, 33, 14, new TimeSpan(0)),
                ModificationTime = new DateTimeOffset(2016, 11, 12, 19, 34, 15, new TimeSpan(0)),
                AbsolutePath = @"C:\Temp\" + name
            };
        }

        public static File CreateFile(string name, string hash = "hash")
        {
            return CreateFile(name, NodeState.None, hash);
        }

        public static Directory CreateDirectory(string name, NodeState state, params Node[] nodes)
        {
            return new Directory
            {
                Name = name,
                State = state,
                InnerNodes = new List<Node>(nodes)
            };
        }

        public static Directory CreateDirectory(string name, params Node[] nodes)
        {
            return CreateDirectory(name, NodeState.None, nodes);
        }
    }
}