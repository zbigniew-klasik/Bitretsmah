using System;
using Bitretsmah.Core.Models;
using System.IO;

namespace Bitretsmah.Core.Interfaces
{
    public interface ILocalFilesService
    {
        Node GetNodeStructure(string nodePath);

        Stream ReadFileStream(string filePath);

        void WriteFileStream(string filePath, Stream stream);

        bool Exists(string path);

        void DeleteFileOrDirectory(string path);

        void SetCreationTime(string filePath, DateTimeOffset time);

        void SetLastWriteTime(string filePath, DateTimeOffset time);
    }
}