using System;

namespace Bitretsmah.Core.Models
{
    public class RestoreRequest
    {
        public string TargetName { get; set; }
        public bool ComputeHashForEachFile { get; set; }
        public IProgress<BackupProgress> Progress { get; set; }
    }
}