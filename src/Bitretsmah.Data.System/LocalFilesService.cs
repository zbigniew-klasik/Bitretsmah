using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Directory = Bitretsmah.Core.Models.Directory;
using File = Bitretsmah.Core.Models.File;

namespace Bitretsmah.Data.System
{
    public class LocalFilesService : ILocalFilesService
    {
        public async Task<Node> GetNodeStructure(string nodePath)
        {
            var file = new FileInfo(nodePath);
            if (file.Exists)
                return await GetFileStructure(file);

            var directory = new DirectoryInfo(nodePath);
            if (directory.Exists)
                return await GetDirectoryStructure(directory);

            throw new Exception("incorrect path"); // todo
        }

        private async Task<File> GetFileStructure(FileInfo fileInfo)
        {
            return new File
            {
                Name = fileInfo.Name,
                Size = fileInfo.Length,
                CreationTime = new DateTimeOffset(fileInfo.CreationTimeUtc, new TimeSpan(0)),
                ModificationTime = new DateTimeOffset(fileInfo.LastWriteTimeUtc, new TimeSpan(0)),
                Hash = await ComputeFileHash(fileInfo)
            };
        }

        private async Task<Directory> GetDirectoryStructure(DirectoryInfo directoryInfo)
        {
            var directory = new Directory { Name = directoryInfo.Name };

            foreach (var info in directoryInfo.GetFiles())
                directory.Nodes.Add(await GetFileStructure(info));

            foreach (var info in directoryInfo.GetDirectories())
                directory.Nodes.Add(await GetDirectoryStructure(info));

            return directory;
        }

        private Task<string> ComputeFileHash(FileInfo fileInfo)
        {
            // todo compute hash
            return Task.FromResult("hash");
        }
    }
}