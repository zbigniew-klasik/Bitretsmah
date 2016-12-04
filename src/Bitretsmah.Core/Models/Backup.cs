using System;

namespace Bitretsmah.Core.Models
{
    public class Backup
    {
        public string Target { get; set; }

        public Node StructureChange { get; set; }

        public DateTimeOffset CreationTime { get; set; }
    }
}