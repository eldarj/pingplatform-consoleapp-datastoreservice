using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Models
{
    public class DSFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string MimeType { get; set; } = "application/octet-stream";
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime LastModifiedTime { get; set; } = DateTime.Now;
        public DateTime LastAccessTime { get; set; } = DateTime.Now;
        public int DirectoryId { get; set; }
        public virtual DSDirectory Directory { get; set; }
        public int OwnerId { get; set; }
        public virtual Account Owner { get; set; }

    }
}
