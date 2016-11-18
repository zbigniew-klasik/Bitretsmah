using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Ensure.That(initialDirectory.Name).IsEqualTo(change.Name);
            Ensure.That(initialDirectory.AbsolutePath).IsEqualTo(change.AbsolutePath);

            var finalDirectory = initialDirectory.DeepCopy();

            change.InnerNodes.Where(x => x.State == NodeState.Created).ToList().ForEach(x =>
            {
                var createdNode = x.DeepCopy();
                createdNode.SetAllStates(NodeState.None);
                finalDirectory.InnerNodes.Add(createdNode);
            });

            // TODO: modified

            finalDirectory.InnerNodes.RemoveAll(x =>
                change.InnerNodes.Any(y => y.Name == x.Name && y.State == NodeState.Deleted));

            return finalDirectory;
        }
    }
}