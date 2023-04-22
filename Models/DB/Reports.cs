using System;
using System.Collections.Generic;

namespace DST.Models.DB
{
    public partial class Reports
    {
        public int ReportId { get; set; }
        public int AppId { get; set; }
        public int StaffId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? DeletedBy { get; set; }
        public bool? DeletedStatus { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Subject { get; set; }
        public int? ElpsDocId { get; set; }
        public int? AppDocId { get; set; }
        public string DocSource { get; set; }
    }
}
