using Bitretsmah.Core.Models;

namespace Bitretsmah.Core
{
    internal interface INodeChangesFinder
    {
        NodeChange Find(Node initialNode, Node finalNode);
    }

    internal class NodeChangesFinder : INodeChangesFinder
    {
        public NodeChange Find(Node initialNode, Node finalNode)
        {
            if (initialNode is File && finalNode is File)
                return TestFile((File)initialNode, (File)finalNode);

            if (initialNode is Directory && finalNode is Directory)
                return TestDirectory((Directory)initialNode, (Directory)finalNode);

            return null;
        }

        public NodeChange TestFile(File initialFile, File finalFile)
        {
            var change = new NodeChange(NodeState.None, finalFile);
            if (initialFile.Name != finalFile.Name) change.Type = NodeState.Modified;
            if (initialFile.Size != finalFile.Size) change.Type = NodeState.Modified;
            if (initialFile.CreationTime != finalFile.CreationTime) change.Type = NodeState.Modified;
            if (initialFile.ModificationTime != finalFile.ModificationTime) change.Type = NodeState.Modified;
            if (initialFile.Hash != null && finalFile.Hash != null && initialFile.Hash != finalFile.Hash) change.Type = NodeState.Modified;
            return change;
        }

        public NodeChange TestDirectory(Directory initialDirectory, Directory finalDirectory)
        {
            var change = new NodeChange(NodeState.None, finalDirectory);
            if (initialDirectory.Name != finalDirectory.Name) change.Type = NodeState.Modified;

            return change;
        }
    }
}