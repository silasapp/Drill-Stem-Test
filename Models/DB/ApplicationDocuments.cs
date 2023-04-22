﻿using System;
using System.Collections.Generic;

namespace DST.Models.DB
{
    public partial class ApplicationDocuments
    {
        public int AppDocId { get; set; }
        public int ElpsDocTypeId { get; set; }
        public string DocName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool DeleteStatus { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DocType { get; set; }
    }
}
