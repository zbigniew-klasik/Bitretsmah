using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.IO;
using System.Linq;
using Directory = Bitretsmah.Core.Models.Directory;
using File = Bitretsmah.Core.Models.File;
using SystemDirectory = System.IO.Directory;
using SystemFile = System.IO.File;
using SystemPath = System.IO.Path;

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

            throw new DirectoryNotFoundException();
        }

        private File GetFileStructure(FileInfo fileInfo)
        {
            return new File
            {
                Name = fileInfo.Name,
                Size = fileInfo.Length,
                CreationTime = new DateTimeOffset(fileInfo.CreationTime),
                ModificationTime = new DateTimeOffset(fileInfo.LastWriteTime),
                Hash = null,
                State = NodeState.None,
                AbsolutePath = fileInfo.FullName
            };
        }

        private Directory GetDirectoryStructure(DirectoryInfo directoryInfo)
        {
            var directory = new Directory
            {
                Name = directoryInfo.Name,
                State = NodeState.None,
                AbsolutePath = directoryInfo.FullName
            };

            foreach (var info in directoryInfo.GetDirectories())
                directory.InnerNodes.Add(GetDirectoryStructure(info));

            foreach (var info in directoryInfo.GetFiles())
                directory.InnerNodes.Add(GetFileStructure(info));

            directory.InnerNodes = directory.InnerNodes
                                                    .OrderByDescending(x => x.GetType().ToString())
                                                    .ThenBy(x => x.Name)
                                                    .ToList();

            return directory;
        }

        public Stream ReadFileStream(string filePath)
        {
            return SystemFile.OpenRead(filePath);
        }

        public void WriteFileStream(string filePath, Stream stream)
        {
            var directoryPath = SystemPath.GetDirectoryName(filePath);

            if (!SystemDirectory.Exists(directoryPath))
            {
                SystemDirectory.CreateDirectory(directoryPath);
            }

            var tempFilePath = SystemPath.Combine(directoryPath, SystemPath.GetRandomFileName());

            using (var writeStream = SystemFile.OpenWrite(tempFilePath))
            {
                stream.CopyTo(writeStream); // TODO: use CopyToAsync()
                writeStream.Close();
            }
            
            var destinationFile = new FileInfo(filePath);
            if (destinationFile.Exists)
            {
                SystemFile.Replace(tempFilePath, filePath, null, true);
            }
            else
            {
                SystemFile.Move(tempFilePath, filePath);
            }
        }

        public bool Exists(string path)
        {
            return SystemDirectory.Exists(path) || SystemFile.Exists(path);
        }

        public void DeleteFileOrDirectory(string path)
        {
            if (SystemDirectory.Exists(path))
                SystemDirectory.Delete(path, true);

            if (SystemFile.Exists(path))
                SystemFile.Delete(path);
        }
    }
}