using DST.Models.DB;
using System;

namespace DST.RecycleModels
{

    public class RecycleAppDocs
    {
        public ApplicationDocuments _appDocs { get; set; }
        public Staff _staffs { get; set; }
    }


    public class RecycleAppProcess
    {
        public bool DeleteStatus { get; set; }

        public int ProcessID { get; set; }
        public string TypeName { get; set; }
        public string StageName { get; set; }
        public string RoleName { get; set; }
        public string LocationName { get; set; }
        public int Sort { get; set; }
        public string CanPush { get; set; }
        public string CanWork { get; set; }
        public string CanAccept { get; set; }
        public string CanReject { get; set; }
        public string CanReport { get; set; }
        public string CanInspect { get; set; }
        public string CanSchdule { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public int LinkId { get; internal set; }
        public string LinkName { get; internal set; }
        public int? RoleID { get; internal set; }
        public int? LocationID { get; internal set; }
        public string RejectRole { get; internal set; }
        public string FlowType { get; internal set; }
        public string AcceptRole { get; internal set; }
        public string Process { get; internal set; }
        public string PushRole { get; internal set; }
    }


    public class RecycleAppStage
    {
        public int StageID { get; set; }
        public string StageName { get; set; }
        public string ShortName { get; set; }
        public int? Amount { get; set; }
        public int? ServiceCharge { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool DeletedStatus { get; set; }
    }


    public class RecycleAppType
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool DeletedStatus { get; set; }
    }


    public class RecycleStageDoc
    {
        public int StageDocID { get; set; }
        public string DocName { get; set; }
        public string StageName { get; set; }
        public string DocType { get; set; }
        public string DeletedAt { get; set; }
        public string CreatedAt { get; set; }
        public string DeletedBy { get; set; }
        public bool DeletedStatus { get; set; }
        public int TypeStageID { get; set; }
        public string TypeName { get;  set; }
        public int Counter { get;  set; }
    }


    public class RecycleCompany
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeletedStatus { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string CompanyCode { get; set; }
    }


    public class RecycleCountry
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeletedStatus { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }


    public class RecycleAnonymous
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeletedStatus { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string CreatedBy { get; internal set; }
        public string Comment { get; internal set; }
        public string Subject { get; internal set; }
        public string DocSource { get; internal set; }
    }

    public class RecycleSchedule
    {
        public int ID { get; set; }
        public string Comment { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string SchduleType { get; set; }
        public string SchduleLocation { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeletedStatus { get; set; }
        public string DeletedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }


    public class RecycleStaff
    {
        public int ID { get; set; }
        public string StaffEmail { get; set; }
        public string StaffName { get; set; }
        public string Office { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeletedStatus { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }


    public class RecycleTanks
    {
        public string Company { get; set; }
        public string Facilities { get; set; }
        public string TankName { get; set; }
        public decimal TankCapacity { get; set; }
        public string Product { get; set; }
        public double Diameter { get; set; }
        public double Height { get; set; }
        public string Posistion { get; set; }
        public string TankType { get; set; }
        public string HasATG { get; set; }
        public string Decom { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeletedStatus { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int ID { get; set; }
    }
}
