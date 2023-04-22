using System;
using System.Collections.Generic;

namespace DST.Models.DB
{
    public partial class SubmittedDocuments
    {
        public int SubDocId { get; set; }
        public int AppId { get; set; }
        public int AppDocId { get; set; }
        public int? CompElpsDocId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool DeleteStatus { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DocSource { get; set; }
        public bool? IsAddictional { get; set; }
    }
}
