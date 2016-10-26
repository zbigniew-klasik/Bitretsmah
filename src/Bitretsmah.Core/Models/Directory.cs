using System;

namespace Bitretsmah.Core.Models
{
    public class Directory
    {
        public string AbsolutPath { get; set; }
        public DateTime LastDeepBackupDate { get; set; }
        public DateTime LastQuickBackupDate { get; set; }
    }
}