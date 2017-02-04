using Bitretsmah.Core.Models;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bitretsmah.Core
{
    public interface INodeChangesFinder
    {
        Node Find(Node initialNode, Node finalNode);
    }

    public class NodeChangesFinder : INodeChangesFinder
    {
        public Node Find(Node initialNode, Node finalNode)
        {
            Ensure.That(finalNode).IsNotNull();

            // TODO: is that right? refactor!
            // TODO: unit test it
            if (initialNode == null)
            {
                var result = finalNode.DeepCopy();
                result.SetAllStates(NodeState.Created);
                return result;
            }

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

            return modified;
        }

        private bool MarkCreatedNodes(List<Node> initialNodes, List<Node> finalNodes)
        {
            var addedNodes = finalNodes.Where(x => initialNodes.All(y => y.Name != x.Name)).ToList();
            addedNodes.ForEach(x => x.SetAllStates(NodeState.Created));
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

            return modified;
        }

        private bool MarkDeletedNodes(List<Node> initialNodes, List<Node> finalNodes)
        {
            var deletedNodes = initialNodes.Where(x => finalNodes.All(y => y.Name != x.Name)).ToList();
            deletedNodes.ForEach(x => x.SetAllStates(NodeState.Deleted));
            finalNodes.AddRange(deletedNodes);
            return deletedNodes.Any();
        }

        private void RemoveUnchangedNodes(Node node)
        {
            var directory = node as Directory;
            if (directory != null)
            {
                directory.InnerNodes.RemoveAll(x => x.State == NodeState.None);
                directory.InnerNodes.ForEach(RemoveUnchangedNodes);
                directory.InnerNodes = directory.InnerNodes
                                                    .OrderByDescending(x => x.GetType().ToString())
                                                    .ThenBy(x => x.Name)
                                                    .ToList();
            }
        }
    }
}