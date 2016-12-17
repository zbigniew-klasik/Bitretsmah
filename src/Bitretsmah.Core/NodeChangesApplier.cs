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

        Node Apply(Node initialNode, ICollection<Node> changes);

        Node Apply(ICollection<Node> changes);
    }

    public class NodeChangesApplier : INodeChangesApplier
    {
        public Node Apply(Node initialNode, Node change)
        {
            EnsureArg.IsNotNull(initialNode);
            EnsureArg.IsNotNull(change);

            var finalNode = initialNode.DeepCopy();
            var changeCopy = change.DeepCopy();
            ApplyChanges(finalNode, changeCopy);
            return finalNode;
        }

        public Node Apply(Node initialNode, ICollection<Node> changes)
        {
            EnsureArg.IsNotNull(initialNode);
            EnsureArg.HasItems(changes, nameof(changes));

            var finalNode = initialNode;

            foreach (var change in changes)
            {
                finalNode = Apply(finalNode, change);
            }

            return finalNode;
        }

        public Node Apply(ICollection<Node> changes)
        {
            EnsureArg.HasItems(changes, nameof(changes));
            var initialNode = changes.First();
            EnsureArg.IsTrue(initialNode.State == NodeState.Created, "The node state should be 'Created'.");
            initialNode.SetAllStates(NodeState.None);
            var otherChanges = changes.Skip(1).ToList();
            var finalNode = Apply(initialNode, otherChanges);
            return finalNode;
        }

        private void ApplyChanges(Node initialNode, Node change)
        {
            if (initialNode is File && change is File)
            {
                ApplyFileChange((File)initialNode, (File)change);
                return;
            }

            if (initialNode is Directory && change is Directory)
            {
                ApplyDirectoryChange((Directory)initialNode, (Directory)change);
                return;
            }

            throw new InvalidOperationException("Unknown node type.");
        }

        private void ApplyFileChange(File finalNode, File change)
        {
            Ensure.That(finalNode.Name).IsEqualTo(change.Name);
            Ensure.That(finalNode.AbsolutePath).IsEqualTo(change.AbsolutePath);
            Ensure.That(change.State == NodeState.Modified).IsTrue();

            finalNode.Hash = change.Hash;
            finalNode.Size = change.Size;
            finalNode.CreationTime = change.CreationTime;
            finalNode.ModificationTime = change.ModificationTime;
            finalNode.State = NodeState.None;
        }

        private void ApplyDirectoryChange(Directory finalDirectory, Directory change)
        {
            Ensure.That(finalDirectory.Name).IsEqualTo(change.Name);
            Ensure.That(finalDirectory.AbsolutePath).IsEqualTo(change.AbsolutePath);

            change.InnerNodes
                .Where(creatingChange => creatingChange.State == NodeState.Created)
                .ToList().ForEach(creatingChange =>
                {
                    creatingChange.SetAllStates(NodeState.None);
                    finalDirectory.InnerNodes.Add(creatingChange);
                });

            change.InnerNodes
                .Where(modifyingChange => modifyingChange.State == NodeState.Modified)
                .ToList().ForEach(modifyingChange =>
                {
                    var modifiedNode = finalDirectory.InnerNodes.Single(node => node.Name == modifyingChange.Name);
                    ApplyChanges(modifiedNode, modifyingChange);
                });

            finalDirectory.InnerNodes.RemoveAll(x =>
                change.InnerNodes.Any(y => y.Name == x.Name && y.State == NodeState.Deleted));
        }
    }
}