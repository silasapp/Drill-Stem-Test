using System;
using System.Collections.Generic;

namespace DST.Models.DB
{
    public partial class ApplicationType
    {
        public int AppTypeId { get; set; }
        public string TypeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool DeleteStatus { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
