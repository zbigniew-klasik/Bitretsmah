using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.IO;
using Directory = Bitretsmah.Core.Models.Directory;
using File = Bitretsmah.Core.Models.File;

namespace Bitretsmah.Data.System
{
    public class LocalFilesService : ILocalFilesService
    {
        public Node GetNodeStructure(string nodePath)
        {
            var file = new FileInfo(nodePath);
            if (file.Exists)
                return GetFileStructure(file);

            var directory = new DirectoryInfo(nodePath);
            if (directory.Exists)
                return GetDirectoryStructure(directory);

            throw new Exception("incorrect path"); // todo
        }

        private File GetFileStructure(FileInfo fileInfo)
        {
            return new File
            {
                Name = fileInfo.Name,
                Size = fileInfo.Length,
                CreationTime = new DateTimeOffset(fileInfo.CreationTimeUtc, new TimeSpan(0)),
                ModificationTime = new DateTimeOffset(fileInfo.LastWriteTimeUtc, new TimeSpan(0)),
                Hash = null,
                State = NodeState.None,
                AbsolutePath = fileInfo.FullName
            };
        }

        private Directory GetDirectoryStructure(DirectoryInfo directoryInfo)
        {
            var directory = new Directory { Name = directoryInfo.Name };

            foreach (var info in directoryInfo.GetDirectories())
                directory.InnerNodes.Add(GetDirectoryStructure(info));

            foreach (var info in directoryInfo.GetFiles())
                directory.InnerNodes.Add(GetFileStructure(info));

            return directory;
        }
    }
}