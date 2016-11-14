﻿using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bitretsmah.Core
{
    internal interface INodeChangesFinder
    {
        Node Find(Node initialNode, Node finalNode);
    }

    internal class NodeChangesFinder : INodeChangesFinder
    {
        public Node Find(Node initialNode, Node finalNode)
        {
            Ensure.That(initialNode).IsNotNull();
            Ensure.That(finalNode).IsNotNull();

            var initialNodeCopy = initialNode.DeepCopy();
            var finalNodeCopy = finalNode.DeepCopy();

            MarkNodeState(initialNodeCopy, finalNodeCopy);
            RemoveUnchangedNodes(finalNodeCopy);
            return finalNodeCopy;
        }

        private bool MarkNodeState(Node initialNode, Node finalNode)
        {
            if (initialNode is File && finalNode is File)
                return MarkFileState((File)initialNode, (File)finalNode);

            if (initialNode is Directory && finalNode is Directory)
                return MarkDirectoryState((Directory)initialNode, (Directory)finalNode);

            throw new InvalidOperationException("Unknown node type.");
        }

        private bool MarkFileState(File initialFile, File finalFile)
        {
            Ensure.That(finalFile.Name).IsEqualTo(initialFile.Name);
            Ensure.That(finalFile.State == NodeState.None).IsTrue();

            var modified = false;

            modified |= (initialFile.Size != finalFile.Size);
            modified |= (initialFile.CreationTime != finalFile.CreationTime);
            modified |= (initialFile.ModificationTime != finalFile.ModificationTime);
            modified |= (initialFile.Hash != null && finalFile.Hash != null && initialFile.Hash != finalFile.Hash);

            finalFile.State = modified ? NodeState.Modified : NodeState.None;
            return modified;
        }

        private bool MarkDirectoryState(Directory initialDirectory, Directory finalDirectory)
        {
            Ensure.That(finalDirectory.Name).IsEqualTo(finalDirectory.Name);
            Ensure.That(finalDirectory.State == NodeState.None).IsTrue();

            var modified = false;

            modified |= MarkCreatedNodes(initialDirectory.InnerNodes, finalDirectory.InnerNodes);
            modified |= MarkModifiedNodes(initialDirectory.InnerNodes, finalDirectory.InnerNodes);
            modified |= MarkDeletedNodes(initialDirectory.InnerNodes, finalDirectory.InnerNodes);

            finalDirectory.State = modified ? NodeState.Modified : NodeState.None;

            // TODO for each inner node check it
            return modified;
        }

        private bool MarkCreatedNodes(List<Node> initialNodes, List<Node> finalNodes)
        {
            var addedNodes = finalNodes.Where(x => initialNodes.All(y => y.Name != x.Name)).ToList();
            addedNodes.ForEach(x => MarkNodeAndAllDescendants(x, NodeState.Created));
            return addedNodes.Any();
        }

        private bool MarkModifiedNodes(List<Node> initialNodes, List<Node> finalNodes)
        {
            var sameFinalNodes = finalNodes.Where(x => initialNodes.Any(y => y.Name == x.Name)).ToList();

            var modified = false;

            foreach (var finalNode in sameFinalNodes)
            {
                var initalNode = initialNodes.Single(y => y.Name == finalNode.Name);
                modified |= MarkNodeState(initalNode, finalNode);
            }

            // todo: test inner directories

            return modified;
        }

        private bool MarkDeletedNodes(List<Node> initialNodes, List<Node> finalNodes)
        {
            var deletedNodes = initialNodes.Where(x => finalNodes.All(y => y.Name != x.Name)).ToList();
            deletedNodes.ForEach(x => MarkNodeAndAllDescendants(x, NodeState.Deleted));
            finalNodes.AddRange(deletedNodes);
            return deletedNodes.Any();
        }

        private void MarkNodeAndAllDescendants(Node node, NodeState state)
        {
            node.State = state;
            (node as Directory)?.InnerNodes.ForEach(x => MarkNodeAndAllDescendants(x, state));
        }

        private void RemoveUnchangedNodes(Node node)
        {
            var directory = node as Directory;
            if (directory != null)
            {
                directory.InnerNodes.RemoveAll(x => x.State == NodeState.None);
                directory.InnerNodes.ForEach(RemoveUnchangedNodes);
            }
        }
    }
}