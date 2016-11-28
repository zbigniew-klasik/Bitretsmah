using System;

namespace Bitretsmah.Core.Models
{
    public class Backup
    {
        public string Target { get; set; }

        public Node Change { get; set; }

        public DateTimeOffset CreationTime { get; set; }
    }
}