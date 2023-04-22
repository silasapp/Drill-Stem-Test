using System;
using System.Collections.Generic;

namespace DST.Models.DB
{
    public partial class Messages
    {
        public int MessageId { get; set; }
        public int CompanyId { get; set; }
        public int AppId { get; set; }
        public string Subject { get; set; }
        public string MesgContent { get; set; }
        public bool Seen { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
