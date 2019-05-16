using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Models
{
    public class DSDirectory
    {
        public int Id { get; set; }
        public int NodeId { get; set; }
        public virtual DSNode Node { get; set; }

        // Parent Directory nav prop
        public int? ParentDirectoryId { get; set; }
        public virtual DSDirectory ParentDirectory { get; set; }
        public virtual ICollection<DSFile> Files { get; set; }
        public virtual ICollection<DSDirectory> Directories { get; set; }
    }
}
