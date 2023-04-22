using System;
using System.Collections.Generic;

namespace DST.Models.DB
{
    public partial class FieldOffices
    {
        public int FieldOfficeId { get; set; }
        public string OfficeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool DeleteStatus { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
