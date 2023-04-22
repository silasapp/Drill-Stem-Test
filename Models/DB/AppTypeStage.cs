using System;
using System.Collections.Generic;

namespace DST.Models.DB
{
    public partial class AppTypeStage
    {
        public int TypeStageId { get; set; }
        public int AppTypeId { get; set; }
        public int AppStageId { get; set; }
        public int Counter { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool DeleteStatus { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
