using System;

namespace Bitretsmah.Core.Models
{
    public class Backup
    {
        public string Path { get; set; }

        public Node Node { get; set; }

        public DateTimeOffset CreationTime { get; set; }
    }
}