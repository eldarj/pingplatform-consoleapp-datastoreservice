using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Models
{
    public enum NodeType
    {
        Directory,
        File
    }

    public class DSNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public bool Private { get; set; } = true;
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime LastModifiedTime { get; set; } = DateTime.Now;

        // Enum type
        public NodeType NodeType { get; set; }

        // Owner nav prop
        public int OwnerId { get; set; }
        public virtual Account Owner { get; set; } // solve the issue of having this in both DSDirectory and DSFile
    }
}
