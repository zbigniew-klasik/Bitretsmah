using Bitretsmah.Core.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Bitretsmah.Data.System
{
    public class FileHashProvider : IFileHashProvider
    {
        public string ComputeFileHash(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            using (var stream = fileInfo.Open(FileMode.Open))
            {
                stream.Position = 0;
                var sha1 = new SHA1Managed();
                var hash = sha1.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}