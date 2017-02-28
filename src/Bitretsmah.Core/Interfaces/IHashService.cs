using System;
using System.Threading.Tasks;
using Bitretsmah.Core.Models;

namespace Bitretsmah.Core.Interfaces
{
    public interface IHashService
    {
        string ComputeFileHash(string filePath);

        Task TryEnsureEachFileHasComputedHash(Node node, IProgress<BackupProgress> progress);

        Task TryEnsureFileHasComputedHash(Core.Models.File file, IProgress<BackupProgress> progress);
    }
}