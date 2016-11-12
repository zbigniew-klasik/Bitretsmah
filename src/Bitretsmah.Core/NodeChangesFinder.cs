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
            var change = new NodeChange(ChangeType.None, finalFile);
            if (initialFile.Name != finalFile.Name) change.Type = ChangeType.Modify;
            if (initialFile.Size != finalFile.Size) change.Type = ChangeType.Modify;
            if (initialFile.CreationTime != finalFile.CreationTime) change.Type = ChangeType.Modify;
            if (initialFile.ModificationTime != finalFile.ModificationTime) change.Type = ChangeType.Modify;
            if (initialFile.Hash != null && finalFile.Hash != null && initialFile.Hash != finalFile.Hash) change.Type = ChangeType.Modify;
            return change;
        }

        public NodeChange TestDirectory(Directory initialDirectory, Directory finalDirectory)
        {
            var change = new NodeChange(ChangeType.None, finalDirectory);
            if (initialDirectory.Name != finalDirectory.Name) change.Type = ChangeType.Modify;
            return change;
        }
    }
}