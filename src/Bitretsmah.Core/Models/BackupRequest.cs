using System;

namespace Bitretsmah.Core.Models
{
    public class BackupRequest
    {
        public string Target { get; set; }
        public string LocalPath { get; set; }
        public bool ComputeHashForEachFile { get; set; }
        public IProgress<BackupProgress> Progress { get; set; }
    }
}