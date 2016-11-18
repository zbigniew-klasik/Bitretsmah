using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Collections.Generic;

namespace Bitretsmah.Core
{
    public interface INodeChangesApplier
    {
        Node Apply(Node initialNode, Node change);

        Node Apply(Node initialNode, IEnumerable<Node> changes);
    }

    public class NodeChangesApplier : INodeChangesApplier
    {
        public Node Apply(Node initialNode, IEnumerable<Node> changes)
        {
            throw new NotImplementedException();
        }

        public Node Apply(Node initialNode, Node change)
        {
            if (initialNode is File && change is File)
                return ApplyFileChange((File)initialNode, (File)change);

            if (initialNode is Directory && change is Directory)
                return ApplyDirectoryChange((Directory)initialNode, (Directory)change);

            throw new InvalidOperationException("Unknown node type.");
        }

        private Node ApplyFileChange(File initialFile, File change)
        {
            Ensure.That(initialFile.Name).IsEqualTo(change.Name);
            Ensure.That(initialFile.AbsolutePath).IsEqualTo(change.AbsolutePath);
            Ensure.That(change.State == NodeState.Modified).IsTrue();

            var finalNode = initialFile.DeepCopy();

            finalNode.Hash = change.Hash;
            finalNode.Size = change.Size;
            finalNode.CreationTime = change.CreationTime;
            finalNode.ModificationTime = change.ModificationTime;
            finalNode.State = NodeState.None;

            return finalNode;
        }

        private Node ApplyDirectoryChange(Directory initialDirectory, Directory change)
        {
            throw new NotImplementedException();
        }
    }
}