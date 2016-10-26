using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Models
{
    public class File
    {
        public string RelativePath { get; set; }
        public string Hash { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime DeletedDate { get; set; }
        public DateTime UploadedDate { get; set; }
        public string Size { get; set; }
        public RemoteId RemoteId { get; set; }
    }
}