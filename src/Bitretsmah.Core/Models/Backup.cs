using System;

namespace Bitretsmah.Core.Models
{
    public class Backup
    {
        public int Id { get; set; }
        public string TargetName { get; set; }
        public Node StructureChange { get; set; }
        public DateTimeOffset CreationTime { get; set; }
    }
}