using Bitretsmah.Core.Models;
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
            // todo: deepcopy because we will be removing

            if (initialNode is File && finalNode is File)
                MarkFile((File)initialNode, (File)finalNode);

            if (initialNode is Directory && finalNode is Directory)
                MarkDirectory((Directory)initialNode, (Directory)finalNode);

            return finalNode;
        }

        public void MarkFile(File initialFile, File finalFile)
        {
            if (initialFile.Name != finalFile.Name) throw new ArgumentException("Files should have the same name to be compared.");
            finalFile.State = NodeState.None;
            if (initialFile.Size != finalFile.Size) finalFile.State = NodeState.Modified;
            if (initialFile.CreationTime != finalFile.CreationTime) finalFile.State = NodeState.Modified;
            if (initialFile.ModificationTime != finalFile.ModificationTime) finalFile.State = NodeState.Modified;
            if (initialFile.Hash != null && finalFile.Hash != null && initialFile.Hash != finalFile.Hash) finalFile.State = NodeState.Modified;
        }

        public void MarkDirectory(Directory initialDirectory, Directory finalDirectory)
        {
            if (initialDirectory.Name != finalDirectory.Name) throw new ArgumentException("Directories should have the same name to be compared.");

            MarkCreatedNodes(initialDirectory.InnerNodes, finalDirectory.InnerNodes);
            MarkModifiedNodes(initialDirectory.InnerNodes, finalDirectory.InnerNodes);
            MarkDeletedNodes(initialDirectory.InnerNodes, finalDirectory.InnerNodes);

            // for each inner node check it
        }

        public void MarkCreatedNodes(List<Node> initialNodes, List<Node> finalNodes)
        {
            var addedNodes = finalNodes.Where(x => initialNodes.All(y => y.Name != x.Name)).ToList();
            addedNodes.ForEach(x => x.State = NodeState.Created);

            // todo mark inner elements creted
        }

        public void MarkModifiedNodes(List<Node> initialNodes, List<Node> finalNodes)
        {
            var sameFiles = finalNodes.Where(x => initialNodes.Any(y => y.Name == x.Name)).OfType<File>().ToList();
            sameFiles.ForEach(x => MarkFile((File)initialNodes.Single(y => y.Name == x.Name), x));

            // todo: test inner directories
        }

        public void MarkDeletedNodes(List<Node> initialNodes, List<Node> finalNodes)
        {
            var deletedNodes = initialNodes.Where(x => finalNodes.All(y => y.Name != x.Name)).ToList();
            deletedNodes.ForEach(x => x.State = NodeState.Deleted);
            // todo mark inner elements deleted
            finalNodes.AddRange(deletedNodes);
        }
    }
}