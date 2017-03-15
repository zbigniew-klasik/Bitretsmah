using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.IO;
using System.Linq;
using Directory = Bitretsmah.Core.Models.Directory;
using File = Bitretsmah.Core.Models.File;
using SystemDirectoryInfo = System.IO.DirectoryInfo;
using SystemFileInfo = System.IO.FileInfo;

namespace Bitretsmah.Data.System
{
    public class LocalFilesService : ILocalFilesService
    {
        public Node GetNodeStructure(string nodePath)
        {
            var file = new SystemFileInfo(nodePath);
            if (file.Exists)
                return GetFileStructure(file);

            var directory = new SystemDirectoryInfo(nodePath);
            if (directory.Exists)
                return GetDirectoryStructure(directory);

            throw new DirectoryNotFoundException();
        }

        private File GetFileStructure(SystemFileInfo fileInfo)
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

        private Directory GetDirectoryStructure(SystemDirectoryInfo directoryInfo)
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
            var fileInfo = new SystemFileInfo(filePath);
            return fileInfo.OpenRead();
        }

        public void WriteFileStream(string filePath, Stream stream)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            var directory = new SystemDirectoryInfo(directoryPath);

            if (!directory.Exists)
            {
                directory.Create();
            }

            var tempFilePath = Path.Combine(directoryPath, Path.GetRandomFileName());
            var tempFile = new SystemFileInfo(tempFilePath);

            using (var writeStream = tempFile.OpenWrite())
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(writeStream);
                writeStream.Close();
            }
            
            var destinationFile = new FileInfo(filePath);
            if (destinationFile.Exists)
            {
                tempFile.Replace(filePath, null, true);
            }
            else
            {
                tempFile.MoveTo(filePath);
            }
        }

        public bool Exists(string path)
        {
            return new SystemDirectoryInfo(path).Exists || new SystemFileInfo(path).Exists;
        }

        public void DeleteFileOrDirectory(string path)
        {
            var directory = new SystemDirectoryInfo(path);
            if (directory.Exists)
                directory.Delete(true);

            var file = new SystemFileInfo(path);
            if (file.Exists)
                file.Delete();
        }
    }
}