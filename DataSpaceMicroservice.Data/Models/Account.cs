using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string DataSpaceDirName { get; set; }
        public virtual ICollection<DSFile> OwningFiles { get; set; }
        public virtual ICollection<FileAccountShare> SharedFiles { get; set; }
    }
}
