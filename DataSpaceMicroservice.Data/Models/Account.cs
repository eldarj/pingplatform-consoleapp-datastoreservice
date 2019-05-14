using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string PhoneNumber { get; set; }
        public string AvatarImageUrl { get; set; }
        public string DataSpaceDirName { get; set; }

        public virtual ICollection<DSNode> OwningNodes { get; set; }
        //public virtual ICollection<DSFile> OwningFiles { get; set; }
        public virtual ICollection<FileAccountShare> SharedFiles { get; set; }
        //public virtual ICollection<DSDirectory> OwningDirectories { get; set; }
    }
}
