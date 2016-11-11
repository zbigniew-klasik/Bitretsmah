using System;

namespace Bitretsmah.Core.Models
{
    public class File : Node
    {
        public string Name { get; set; }
        public decimal Size { get; set; }
        public string Hash { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset ModificationTime { get; set; }
    }
}