using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Models
{
    public class Directory
    {
        public string AbsolutPath { get; set; }
        public DateTime LastDeepBackupDate { get; set; }
        public DateTime LastQuickBackupDate { get; set; }
    }
}