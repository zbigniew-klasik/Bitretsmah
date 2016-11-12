﻿using System;

namespace Bitretsmah.Core.Models
{
    public class File : Node
    {
        public File()
        {
        }

        public File(string name, decimal size, string hash, DateTimeOffset creationTime, DateTimeOffset modificationTime)
        {
            Name = name;
            Size = size;
            Hash = hash;
            CreationTime = creationTime;
            ModificationTime = modificationTime;
        }

        public decimal Size { get; set; }
        public string Hash { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset ModificationTime { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}