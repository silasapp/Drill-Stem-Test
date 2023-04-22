﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DST.Models.DB
{
    public partial class DST_DBContext : DbContext
    {
        public DST_DBContext()
        {
        }

        public DST_DBContext(DbContextOptions<DST_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppDeskHistory> AppDeskHistory { get; set; }
        public virtual DbSet<AppStageDocuments> AppStageDocuments { get; set; }
        public virtual DbSet<AppTypeStage> AppTypeStage { get; set; }
        public virtual DbSet<ApplicationDocuments> ApplicationDocuments { get; set; }
        public virtual DbSet<ApplicationProccess> ApplicationProccess { get; set; }
        public virtual DbSet<ApplicationStage> ApplicationStage { get; set; }
        public virtual DbSet<ApplicationType> ApplicationType { get; set; }
        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<AuditTrail> AuditTrail { get; set; }
        public virtual DbSet<Companies> Companies { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Facilities> Facilities { get; set; }
        public virtual DbSet<FieldOffices> FieldOffices { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Logins> Logins { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<MyDesk> MyDesk { get; set; }
        public virtual DbSet<NominatedStaff> NominatedStaff { get; set; }
        public virtual DbSet<NominationRequest> NominationRequest { get; set; }
        public virtual DbSet<OutOfOffice> OutOfOffice { get; set; }
        public virtual DbSet<PermitHistory> PermitHistory { get; set; }
        public virtual DbSet<Permits> Permits { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<Schdules> Schdules { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<SubmittedDocuments> SubmittedDocuments { get; set; }
        public virtual DbSet<TemplateTable> TemplateTable { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<ZonalOffice> ZonalOffice { get; set; }
        public virtual DbSet<ZoneFieldOffice> ZoneFieldOffice { get; set; }
        public virtual DbSet<ZoneStates> ZoneStates { get; set; }


//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=LAPTOP-359CN48E;Database=DST_DB;Trusted_Connection=True;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppDeskHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId);

                entity.Property(e => e.HistoryId).HasColumnName("HistoryID");

                entity.Property(e => e.ActionFrom)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ActionTo)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AppStageDocuments>(entity =>
            {
                entity.HasKey(e => e.StageDocId);

                entity.Property(e => e.StageDocId).HasColumnName("StageDocID");

                entity.Property(e => e.AppDocId).HasColumnName("AppDocID");

                entity.Property(e => e.AppStageId).HasColumnName("AppStageID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<AppTypeStage>(entity =>
            {
                entity.HasKey(e => e.TypeStageId)
                    .HasName("PK__AppTypeS__10C1BC3578DB58AC");

                entity.Property(e => e.TypeStageId).HasColumnName("TypeStageID");

                entity.Property(e => e.AppStageId).HasColumnName("AppStageID");

                entity.Property(e => e.AppTypeId).HasColumnName("AppTypeID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ApplicationDocuments>(entity =>
            {
                entity.HasKey(e => e.AppDocId);

                entity.Property(e => e.AppDocId).HasColumnName("AppDocID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DocName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DocType)
                    .HasColumnName("docType")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.ElpsDocTypeId).HasColumnName("ElpsDocTypeID");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ApplicationProccess>(entity =>
            {
                entity.HasKey(e => e.ProccessId)
                    .HasName("PK_ApplicationProccess_1");

                entity.Property(e => e.ProccessId).HasColumnName("ProccessID");

                entity.Property(e => e.CanAccept).HasColumnName("canAccept");

                entity.Property(e => e.CanInspect).HasColumnName("canInspect");

                entity.Property(e => e.CanPush).HasColumnName("canPush");

                entity.Property(e => e.CanReject).HasColumnName("canReject");

                entity.Property(e => e.CanReport).HasColumnName("canReport");

                entity.Property(e => e.CanSchdule).HasColumnName("canSchdule");

                entity.Property(e => e.CanWork).HasColumnName("canWork");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.OnAcceptRoleId).HasColumnName("onAcceptRoleID");

                entity.Property(e => e.OnRejectRoleId).HasColumnName("onRejectRoleID");

                entity.Property(e => e.Process)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.StageId).HasColumnName("StageID");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ApplicationStage>(entity =>
            {
                entity.HasKey(e => e.AppStageId);

                entity.Property(e => e.AppStageId).HasColumnName("AppStageID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StageName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ApplicationType>(entity =>
            {
                entity.HasKey(e => e.AppTypeId)
                    .HasName("PK__Applicat__141296A271104D0E");

                entity.Property(e => e.AppTypeId).HasColumnName("AppTypeID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Applications>(entity =>
            {
                entity.HasKey(e => e.AppId)
                    .HasName("PK__Applicat__8E2CF7F935DD47F0");

                entity.Property(e => e.AppRefNo)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.AppTypeStageId).HasColumnName("AppTypeStageID");

                entity.Property(e => e.Comment)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Contractor)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CurrentDeskId).HasColumnName("CurrentDeskID");

                entity.Property(e => e.DateApplied).HasColumnType("datetime");

                entity.Property(e => e.DateSubmitted).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DeskId).HasColumnName("DeskID");

                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");

                entity.Property(e => e.IsProposedSubmitted).HasColumnName("isProposedSubmitted");

                entity.Property(e => e.RigName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RigType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Volume).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<AuditTrail>(entity =>
            {
                entity.HasKey(e => e.AuditLogId);

                entity.Property(e => e.AuditLogId).HasColumnName("AuditLogID");

                entity.Property(e => e.AuditAction)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Companies>(entity =>
            {
                entity.HasKey(e => e.CompanyId);

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Avarta)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyElpsId).HasColumnName("CompanyElpsID");

                entity.Property(e => e.CompanyEmail)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.IdentificationCode)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.IsFirstTime).HasColumnName("isFirstTime");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.StateName)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Countries>(entity =>
            {
                entity.HasKey(e => e.CountryId);

                entity.Property(e => e.CountryId).HasColumnName("Country_id");

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Facilities>(entity =>
            {
                entity.HasKey(e => e.FacilityId)
                    .HasName("PK_Facility");

                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");

                entity.Property(e => e.City)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.ContactName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPhone)
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.ElpsFacilityId).HasColumnName("ElpsFacilityID");

                entity.Property(e => e.FacilityAddress)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.FacilityName)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.IsHighTention).HasColumnName("isHighTention");

                entity.Property(e => e.IsHighWay).HasColumnName("isHighWay");

                entity.Property(e => e.IsPipeLine).HasColumnName("isPipeLine");

                entity.Property(e => e.Lga)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<FieldOffices>(entity =>
            {
                entity.HasKey(e => e.FieldOfficeId)
                    .HasName("PK_FieldOffice");

                entity.Property(e => e.FieldOfficeId).HasColumnName("FieldOffice_id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.OfficeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.LocationName)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Logins>(entity =>
            {
                entity.HasKey(e => e.LoginId);

                entity.Property(e => e.LoginId).HasColumnName("LoginID");

                entity.Property(e => e.HostName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.LocalIp)
                    .HasColumnName("Local_Ip")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.LoginStatus)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.LoginTime).HasColumnType("datetime");

                entity.Property(e => e.LogoutTime).HasColumnType("datetime");

                entity.Property(e => e.MacAddress)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RemoteIp)
                    .HasColumnName("Remote_Ip")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<Messages>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.MesgContent)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(600)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MyDesk>(entity =>
            {
                entity.HasKey(e => e.DeskId);

                entity.Property(e => e.DeskId).HasColumnName("DeskID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.Comment).IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.HasPushed).HasColumnName("hasPushed");

                entity.Property(e => e.HasWork).HasColumnName("hasWork");

                entity.Property(e => e.ProcessId).HasColumnName("ProcessID");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<NominatedStaff>(entity =>
            {
                entity.HasKey(e => e.NominateId)
                    .HasName("PK__Nominate__B7E977CFB6FEC1BE");

                entity.Property(e => e.NominateId).HasColumnName("NominateID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.Contents).IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Designation)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DocSource)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.HasSubmitted).HasColumnName("hasSubmitted");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RespondComment)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.RespondStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.SubmittedAt).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<NominationRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PK__Nominati__33A8517AF474175E");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.HasDone).HasColumnName("hasDone");

                entity.Property(e => e.ReminderDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<OutOfOffice>(entity =>
            {
                entity.HasKey(e => e.OutId);

                entity.Property(e => e.OutId).HasColumnName("OutID");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateFrom).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.ReliverId).HasColumnName("ReliverID");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<PermitHistory>(entity =>
            {
                entity.Property(e => e.PermitHistoryId).HasColumnName("PermitHistoryID");

                entity.Property(e => e.DownloadedAt).HasColumnType("datetime");

                entity.Property(e => e.PermitId).HasColumnName("PermitID");

                entity.Property(e => e.PreviewedAt).HasColumnType("datetime");

                entity.Property(e => e.UserDetails)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ViewType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Permits>(entity =>
            {
                entity.HasKey(e => e.PermitId);

                entity.Property(e => e.PermitId).HasColumnName("PermitID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.IsRenewed).HasColumnName("isRenewed");

                entity.Property(e => e.IssuedDate).HasColumnType("datetime");

                entity.Property(e => e.PermitElpsId).HasColumnName("PermitElpsID");

                entity.Property(e => e.PermitNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Reports>(entity =>
            {
                entity.HasKey(e => e.ReportId);

                entity.Property(e => e.ReportId).HasColumnName("ReportID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DocSource)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Schdules>(entity =>
            {
                entity.HasKey(e => e.SchduleId);

                entity.Property(e => e.SchduleId).HasColumnName("SchduleID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.Comment).IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CustomerComment).IsUnicode(false);

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.SchduleDate).HasColumnType("datetime");

                entity.Property(e => e.SchduleLocation)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.SchduleType)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.SupervisorComment).IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.FieldOfficeId).HasColumnName("FieldOfficeID");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.SignatureName)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.SignaturePath)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.StaffElpsId)
                    .IsRequired()
                    .HasColumnName("StaffElpsID")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.StaffEmail)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Theme)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.HasKey(e => e.StateId);

                entity.Property(e => e.StateId).HasColumnName("State_id");

                entity.Property(e => e.CountryId).HasColumnName("Country_id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.StateName)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<SubmittedDocuments>(entity =>
            {
                entity.HasKey(e => e.SubDocId);

                entity.Property(e => e.SubDocId).HasColumnName("SubDocID");

                entity.Property(e => e.AppDocId).HasColumnName("AppDocID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.CompElpsDocId).HasColumnName("CompElpsDocID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DocSource).IsUnicode(false);

                entity.Property(e => e.IsAddictional).HasColumnName("isAddictional");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplateTable>(entity =>
            {
                entity.HasKey(e => e.TempId)
                    .HasName("PK__Template__06C703C1F0BE9A82");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.FieldName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OmlOpl)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.String)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.FluidType)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DriveMechanism)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Reservior)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Terrian)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.WellName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.ElpsTransId).HasColumnName("ElpsTransID");

                entity.Property(e => e.Rrr)
                    .HasColumnName("RRR")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.TransRef)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionStatus)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId).HasColumnName("Role_id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ZonalOffice>(entity =>
            {
                entity.HasKey(e => e.ZoneId)
                    .HasName("PK_Zones");

                entity.Property(e => e.ZoneId).HasColumnName("Zone_id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.ZoneName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ZoneFieldOffice>(entity =>
            {
                entity.Property(e => e.ZoneFieldOfficeId).HasColumnName("ZoneFieldOffice_id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.FieldOfficeId).HasColumnName("FieldOffice_id");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.ZoneId).HasColumnName("Zone_id");
            });

            modelBuilder.Entity<ZoneStates>(entity =>
            {
                entity.Property(e => e.ZoneStatesId).HasColumnName("ZoneStates_id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.StateId).HasColumnName("State_id");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.ZoneId).HasColumnName("Zone_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
