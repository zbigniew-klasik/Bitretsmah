using System;

namespace Bitretsmah.Core.Models
{
    [Serializable]
    public class File : Node
    {
        public decimal Size { get; set; }
        public string Hash { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset ModificationTime { get; set; }
    }
}