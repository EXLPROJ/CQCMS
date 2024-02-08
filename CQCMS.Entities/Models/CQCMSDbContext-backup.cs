//using System;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity;
//using System.Linq;

//namespace CQCMS.EmailApp.Models
//{
//    public partial class CQCMSDbContext : DbContext
//    {
//        public CQCMSDbContext()
//            : base("name=CQCMSDbContext")
//        {
//        }

//        public virtual DbSet<AppConfiguration> AppConfigurations { get; set; }
//        public virtual DbSet<AuditTrail> AuditTrails { get; set; }
//        public virtual DbSet<BackupUserDetail> BackupUserDetails { get; set; }
//        public virtual DbSet<CaseAuditTrail> CaseAuditTrails { get; set; }
//        public virtual DbSet<CaseStatusLookup> CaseStatusLookups { get; set; }
//        public virtual DbSet<Category> Categories { get; set; }
//        public virtual DbSet<CategoryLookup> CategoryLookups { get; set; }
//        public virtual DbSet<EmailAttachment> EmailAttachments { get; set; }
//        public virtual DbSet<EmailBase> EmailBases { get; set; }
//        public virtual DbSet<EmailHTMLBody> EmailHTMLBodies { get; set; }
//        public virtual DbSet<EmailTextBody> EmailTextBodies { get; set; }
//        public virtual DbSet<EmailType> EmailTypes { get; set; }
//        public virtual DbSet<HolidayList> HolidayLists { get; set; }
//        public virtual DbSet<SubCategory> SubCategories { get; set; }
//        public virtual DbSet<UserDetail> UserDetails { get; set; }
//        public virtual DbSet<UserToManagerMapping> UserToManagerMappings { get; set; }
//        public virtual DbSet<CaseDetail> CaseDetails { get; set; }
//        public virtual DbSet<CaseIdentifierMaxCount> CaseIdentifierMaxCounts { get; set; }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<AppConfiguration>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<AuditTrail>()
//                .Property(e => e.Action)
//                .IsUnicode(false);

//            modelBuilder.Entity<AuditTrail>()
//                .Property(e => e.UserID)
//                .IsUnicode(false);

//            modelBuilder.Entity<AuditTrail>()
//                .Property(e => e.RowID)
//                .IsUnicode(false);

//            modelBuilder.Entity<AuditTrail>()
//                .Property(e => e.Module)
//                .IsUnicode(false);

//            modelBuilder.Entity<AuditTrail>()
//                .Property(e => e.Field)
//                .IsUnicode(false);

//            modelBuilder.Entity<AuditTrail>()
//                .Property(e => e.Oldvalue)
//                .IsUnicode(false);

//            modelBuilder.Entity<AuditTrail>()
//                .Property(e => e.NewValue)
//                .IsUnicode(false);

//            modelBuilder.Entity<BackupUserDetail>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseAuditTrail>()
//                .Property(e => e.Action)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseAuditTrail>()
//                .Property(e => e.ActedBy)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseAuditTrail>()
//                .Property(e => e.Comment)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseStatusLookup>()
//                .Property(e => e.CaseStatus)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseStatusLookup>()
//                .Property(e => e.StatusType)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseStatusLookup>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.CategoryKeywords)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.SLA)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.TouchTAT)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.PostSLATouchTAT)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.Complexity)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.ProcessingTime)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.MLThreshold)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .HasMany(e => e.SubCategories)
//                .WithRequired(e => e.Category)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<CategoryLookup>()
//                .Property(e => e.Incident)
//                .IsUnicode(false);

//            modelBuilder.Entity<CategoryLookup>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<EmailAttachment>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<EmailAttachment>()
//                .Property(e => e.LastActedBy)
//                .IsUnicode(false);

//            modelBuilder.Entity<EmailBase>()
//                .Property(e => e.EmailDirection)
//                .IsUnicode(false);

//            modelBuilder.Entity<EmailBase>()
//                .Property(e => e.Priority)
//                .IsUnicode(false);

//            modelBuilder.Entity<EmailBase>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<EmailBase>()
//                .Property(e => e.EmailHash)
//                .IsUnicode(false);

//            modelBuilder.Entity<EmailType>()
//                .Property(e => e.EmailType1)
//                .IsUnicode(false);

//            modelBuilder.Entity<EmailType>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.SubCategoryKeywords)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.Encode)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.SLA)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.TouchTAT)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.PostSLATouchTAT)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.Complexity)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.ProcessingTime)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<SubCategory>()
//                .Property(e => e.MLThreshold)
//                .IsUnicode(false);

//            modelBuilder.Entity<UserDetail>()
//                .Property(e => e.WeightedCapacity)
//                .HasPrecision(8, 2);

//            modelBuilder.Entity<UserDetail>()
//                .Property(e => e.ShortCode)
//                .IsUnicode(false);

//            modelBuilder.Entity<UserDetail>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<UserDetail>()
//                .Property(e => e.ShiftInTime)
//                .HasPrecision(0);

//            modelBuilder.Entity<UserDetail>()
//                .Property(e => e.ShiftOutTime)
//                .HasPrecision(0);

//            modelBuilder.Entity<UserToManagerMapping>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.CurrentlyAssignedTo)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.PreviouslyAssignedTo)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.CIN)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.AccountNumber)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.BusinessSegment)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.BusinessLineCode)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.PendingStatus)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.CaseAdditionalDetail)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.Priority)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.ComplaintOutcome)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.ImpactToClient)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.ComplaintErrorCode)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.ComplaintContactChannel)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.MatchedPartialCases)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.NOQ_1)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.NOQ_2)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.NOQ_3)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.NOQ1_Score)
//                .HasPrecision(12, 9);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.NOQ2_Score)
//                .HasPrecision(12, 9);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.NOQ3_Score)
//                .HasPrecision(12, 9);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.CaseIdIdentifier)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.Country)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.EmailFailureToken)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.CaseEscalatedManager)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.EscalationOriginatorName)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseDetail>()
//                .Property(e => e.MultiAccountNumber)
//                .IsUnicode(false);

//            modelBuilder.Entity<CaseIdentifierMaxCount>()
//                .Property(e => e.Country)
//                .IsUnicode(false);
//        }
//    }
//}
