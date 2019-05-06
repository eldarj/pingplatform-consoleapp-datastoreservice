using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Models
{
    public class FileAccountShare
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        public int FileId { get; set; }
        public virtual DSFile File { get; set; }
    }
}
