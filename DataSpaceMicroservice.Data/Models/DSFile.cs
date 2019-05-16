using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Models
{
    public class DSFile
    {
        public int Id { get; set; }
        public string MimeType { get; set; } = "application/octet-stream";
        public int NodeId { get; set; }
        public virtual DSNode Node { get; set; }

        // Parent Directory nav prop
        public int? ParentDirectoryId { get; set; }
        public virtual DSDirectory ParentDirectory { get; set; }
    }
}
