using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Models
{
    public class DSDirectory
    {
        public int Id { get; set; }
        public string DirName { get; set; }
        public string Path { get; set; }
        public bool Empty { get; set; } = false;
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime LastModifiedTime { get; set; } = DateTime.Now;
        public DateTime LastAccessTime { get; set; } = DateTime.Now;
        public virtual ICollection<DSFile> Files { get; set; }
    }
}
