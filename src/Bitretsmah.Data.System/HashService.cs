using Bitretsmah.Core.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Bitretsmah.Data.System
{
    public class HashService : IHashService
    {
        public string ComputeFileHash(string filePath)
        {
            var file = new FileInfo(filePath);
            if (file.Exists) return ComputeFileHash(file);
            throw new FileNotFoundException();
        }

        internal string ComputeFileHash(FileInfo fileInfo)
        {
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