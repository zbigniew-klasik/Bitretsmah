using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.IO;
using System.Security.Cryptography;
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

        public string ComputeHash(string filePath)
        {
            var file = new FileInfo(filePath);
            if (file.Exists)
                return ComputeHash(file);

            throw new Exception("incorrect path"); // todo
        }

        internal string ComputeHash(FileInfo fileInfo)
        {
            using (var stream = fileInfo.Open(FileMode.Open))
            {
                stream.Position = 0;
                var sha1 = new SHA1Managed();
                var hash = sha1.ComputeHash(stream);
                return Convert.ToBase64String(hash);
            }
        }

        private File GetFileStructure(FileInfo fileInfo)
        {
            return new File
            {
                Name = fileInfo.Name,
                Size = fileInfo.Length,
                CreationTime = new DateTimeOffset(fileInfo.CreationTimeUtc, new TimeSpan(0)),
                ModificationTime = new DateTimeOffset(fileInfo.LastWriteTimeUtc, new TimeSpan(0)),
                Hash = null
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