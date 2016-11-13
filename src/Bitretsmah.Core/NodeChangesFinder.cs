﻿using Bitretsmah.Core.Models;
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
            var initialNodeCopy = initialNode.DeepCopy();
            var finalNodeCopy = finalNode.DeepCopy();

            MarkNodeState(initialNodeCopy, finalNodeCopy);

            // todo: clear not change nodes
            // CleanUp(finalNodeCopy);

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
            if (initialFile.Name != finalFile.Name) throw new ArgumentException("Files should have the same name to be compared.");

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
            if (initialDirectory.Name != finalDirectory.Name) throw new ArgumentException("Directories should have the same name to be compared.");
            // todo initial state should be None!!!

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
            addedNodes.ForEach(x => x.State = NodeState.Created);

            // todo mark inner elements creted

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
            deletedNodes.ForEach(x => x.State = NodeState.Deleted);

            // todo mark inner elements deleted

            finalNodes.AddRange(deletedNodes);
            return deletedNodes.Any();
        }
    }
}