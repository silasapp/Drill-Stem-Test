using System;
using System.Collections.Generic;

namespace DST.Models.DB
{
    public partial class TemplateTable
    {
        public int TempId { get; set; }
        public int? AppId { get; set; }
        public string OmlOpl { get; set; }
        public string FieldName { get; set; }
        public string Reservior { get; set; }
        public string WellName { get; set; }
        public string String { get; set; }
        public string Terrian { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FluidType { get; set; }
        public string DriveMechanism { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
