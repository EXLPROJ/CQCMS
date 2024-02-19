using CQCMS.Entities;
using CQCMS.Entities.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;

namespace CQCMS.Entities.Models
{
    public partial class CQCMSDbContext : DbContext
    {
        public CQCMSDbContext()
            : base("name=CQCMSDbContext")
        {
        }

        public virtual DbSet<BackupUserDetail> BackupUserDetails { get; set; }
       // public virtual DbSet<Rule> Rules { get; set; }

        //public virtual DbSet<InformationSchemaColumn> InformationSchemaColumn { get; set; }
        public virtual DbSet<CaseDetail> CaseDetails { get; set; }
        public virtual DbSet<CaseStatusLookup> CaseStatusLookups { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
       // public virtual DbSet<ClientInfo> ClientInfoes { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<EmailAttachment> Emailattachments { get; set; }
        //public virtual DbSet<EmailClassificationRule> EmailClassificationRules { get; set; }
        public virtual DbSet<EmailType> Emailtypes { get; set; }
        public virtual DbSet<Mailbox> Mailboxes { get; set; }
        public virtual DbSet<MailboxAccess> MailboxAccesses { get; set; }
        //public virtual DbSet<MailboxClientProcessMap> MailboxClientProcessMaps { get; set; }
       // public virtual DbSet<ProcessInfo> ProcessInfoes { get; set; }
        //public virtual DbSet<Signature> Signatures { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
       // public virtual DbSet<SubProcessInfo> SubProcessInfoes { get; set; }
        //public virtual DbSet<Template> Templates { get; set; }
       // public virtual DbSet<TemplateVM> TemplatesVM { get; set; }
        //public virtual DbSet<UserClientProcessMap> UserClientProcessMaps { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }
        //public virtual DbSet<TimeZones> TimeZones { get; set; }
        //public virtual DbSet<QNLockup> QNLookups { get; set; }
       // public virtual DbSet<FileDetails> FileDetails { get; set; }
        //public virtual DbSet<MassContacts> MassContacts { get; set; }
        //public virtual DbSet<SignerInformation> SignerInformations { get; set; }
        //public virtual DbSet<SignerAddress> SignerAddresses { get; set; }
        //public virtual DbSet<SignerUpdateCaseDetails> SignerUpdateCaseDetails { get; set; }
        //public virtual DbSet<SignerUpdateLLCMember> SignerUpdateLLCMembers { get; set; }
        //public virtual DbSet<LegalDocuments> LegalDocuments { get; set; }
        public virtual DbSet<BackupUserDetailVM> BackupUserDetailsvM { get; set; }
        public virtual DbSet<CategoryVM> CategoriesVM { get; set; }
        //public virtual DbSet<ClientInfoVM> ClientInfoesVM { get; set; }
        public virtual DbSet<MailboxVM> MailboxesVM { get; set; }
        public virtual DbSet<MailboxAccessVM> MailboxAccessesVM { get; set; }
        //public virtual DbSet<MailboxClientProcessMapVM> MailboxClientProcessMapsvM { get; set; }
        //public virtual DbSet<ProcessInfoVM> ProcessInfoesVM { get; set; }
        //public virtual DbSet<SignatureVM> SignaturesVM { get; set; }
        public virtual DbSet<SubCategoryVM> SubCategoriesvM { get; set; }
        //public virtual DbSet<SubProcessInfoVM> SubProcessInfoesvM { get; set; }
        //public virtual DbSet<UserClientProcessMapVM> UserClientProcessMapsvM { get; set; }
        public virtual DbSet<UserDetailVM> UserDetailsVM { get; set; }
        //public virtual DbSet<QNLockupVM> QNLookupsvM { get; set; }
        //public virtual DbSet<FileTranslationRules> FileTranslationRules { get; set; }
        //public virtual DbSet<CaseLiveSignSigner> CaseLiveSignSigner { get; set; }
        //public virtual DbSet<SignerGroup> SignerGroups { get; set; }
        //public virtual DbSet<RemoveSigners> RemoveSigners { get; set; }
        //public virtual DbSet<HashFamily> HashFamilyvM { get; set; }
        //public virtual DbSet<HashFamilyDepend> HashFamilyDepend { get; set; }
        //public virtual DbSet<HashTag> HashTag { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        //public virtual DbSet<SignerReportStatus> SignerReportStatus { get; set; }
        //public virtual DbSet<ReportStatusWithCaseID> ReportStatusWithCaseID { get; set; }
        //public virtual DbSet<CaseCustomAttribute> CaseCustomAttribute { get; set; }
        //public virtual DbSet<AdditionalActionConfig> AdditionalActionConfig { get; set; }
        //public virtual DbSet<PreProcessRuleActions> PreProcessRuleActions { get; set; }
        //public virtual DbSet<Ruleoption> RuleOption { get; set; }
        //public virtual DbSet<AdditionalAction> AdditionalAction { get; set; }
        //public virtual DbSet<TemplateToken> TemplateToken { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CaseDetail>()
            .Property(e => e.CurrentlyAssignedTo)
            .IsUnicode(false);

            modelBuilder.Entity<CaseDetail>()
            .Property(e => e.PreviouslyAssignedTo)
            .IsUnicode(false);

            modelBuilder.Entity<CaseDetail>()
            .Property(e => e.CIN)
            .IsUnicode(false);

            modelBuilder.Entity<CaseDetail>()
            .Property(e => e.AccountNumber)
            .IsUnicode(false);

            modelBuilder.Entity<CaseDetail>()
            .Property(e => e.Priority)
            .IsUnicode(false);

            modelBuilder.Entity<CaseStatusLookup>()
            .Property(e => e.CaseStatus)
            .IsUnicode(false);

            modelBuilder.Entity<CaseStatusLookup>()
            .Property(e => e.StatusType)
            .IsUnicode(false);

            modelBuilder.Entity<CaseStatusLookup>()
            .HasMany(e => e.CaseDetails)
            .WithRequired(e => e.CaseStatusLookup)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<CaseStatusLookup>()
            .HasMany(e => e.Emails)
            .WithOptional(e => e.CaseStatusLookup)
            .HasForeignKey(e => e.Emailstatus);

            modelBuilder.Entity<Category>()
            .Property(e => e.CategoryName)
            .IsUnicode(false);

            modelBuilder.Entity<Category>()
            .Property(e => e.CategoryKeywords)
            .IsUnicode(false);

            modelBuilder.Entity<Category>()
            .HasMany(e => e.SubCategories)
            .WithRequired(e => e.Category)
            .WillCascadeOnDelete(false);

            //modelBuilder.Entity<ClientInfo>()
            //.Property(e => e.CIN)
            //.IsUnicode(false);

            //modelBuilder.Entity<ClientInfo>()
            //.Property(e => e.AccountNumber)
            //.IsUnicode(false);

            //modelBuilder.Entity<ClientInfo>()
            //.Property(e => e.MasterGroupName)
            //.IsUnicode(false);

            modelBuilder.Entity<Email>()
            .Property(e => e.EmailSubject)
            .IsUnicode(true);

            modelBuilder.Entity<Email>()
            .Property(e => e.EmailFrom)
            .IsUnicode(true);

            modelBuilder.Entity<Email>()
            .Property(e => e.Emailto)
            .IsUnicode(true);

            modelBuilder.Entity<Email>()
            .Property(e => e.Emailcc)
            .IsUnicode(true);

            modelBuilder.Entity<Email>()
            .Property(e => e.EmailFolder)
            .IsUnicode(true);

            modelBuilder.Entity<Email>()
            .Property(e => e.EmailSubFolder)
            .IsUnicode(true);

            modelBuilder.Entity<Email>()
            .Property(e => e.EmailDirection)
            .IsUnicode(false);

            modelBuilder.Entity<Email>()
            .HasMany(e => e.EmailAttachments)
            .WithRequired(e => e.Email)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<EmailAttachment>()
            .Property(e => e.EmailFileName)
            .IsUnicode(true);

            modelBuilder.Entity<EmailAttachment>()
            .Property(e => e.EmailOriginalFileName)
            .IsUnicode(true);

            modelBuilder.Entity<EmailAttachment>()
            .Property(e => e.EmailFilePath)
            .IsUnicode(true);

            //modelBuilder.Entity<EmailClassificationRule>()
            //.Property(e => e.InputVariable)
            //.IsFixedLength();

            //modelBuilder.Entity<EmailClassificationRule>()
            //.Property(e => e.Additionalattribute)
            //.IsFixedLength();

            modelBuilder.Entity<EmailType>()
            .Property(e => e.EmailType1)
            .IsUnicode(false);

            modelBuilder.Entity<Mailbox>()
            .Property(e => e.MailboxName)
            .IsUnicode(false);

            modelBuilder.Entity<Mailbox>()
            .Property(e => e.MailboxType)
            .IsUnicode(false);

            modelBuilder.Entity<Mailbox>()
            .Property(e => e.MailFolder)
            .IsUnicode(false);

            modelBuilder.Entity<Mailbox>()
            .Property(e => e.MoveToFolder)
            .IsUnicode(false);

            modelBuilder.Entity<Mailbox>()
            .Property(e => e.ExceptionFolder)
            .IsUnicode(false);

            modelBuilder.Entity<Mailbox>()
            .HasMany(e => e.CaseDetails)
            .WithRequired(e => e.Mailbox)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mailbox>()
            .HasMany(e => e.Emails)
            .WithRequired(e => e.Mailbox)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mailbox>()
            .HasMany(e => e.MailboxAccesses)
            .WithOptional(e => e.Mailbox)
            .HasForeignKey(e => e.AllowedMailboxID);

            //modelBuilder.Entity<Mailbox>()
            //.HasMany(e => e.Signatures)
            //.WithRequired(e => e.Mailbox)
            //.WillCascadeOnDelete(false);

            modelBuilder.Entity<Mailbox>()
            .HasMany(e => e.MailboxAccesses1)
            .WithOptional(e => e.Mailbox1)
            .HasForeignKey(e => e.PriorityMailboxID);

            modelBuilder.Entity<Mailbox>()
            .HasMany(e => e.MailboxAccesses2)
            .WithOptional(e => e.Mailbox2)
            .HasForeignKey(e => e.RestrictedMailboxID);

            //modelBuilder.Entity<ProcessInfo>()
            //.Property(e => e.ProcessName)
            //.IsUnicode(false);

            //modelBuilder.Entity<ProcessInfo>()
            //.Property(e => e.ProcessShortCode)
            //.IsUnicode(false);

            modelBuilder.Entity<SubCategory>()
            .Property(e => e.SubCategoryName)
            .IsUnicode(false);

            modelBuilder.Entity<SubCategory>()
            .Property(e => e.SubCategoryKeywords)
            .IsUnicode(false);

            //modelBuilder.Entity<SubProcessInfo>()
            //.Property(e => e.SubProcessName)
            //.IsUnicode(false);

            //modelBuilder.Entity<SubProcessInfo>()
            //.Property(e => e.SubProcessShortCode)
            //.IsUnicode(false);

            //modelBuilder.Entity<UserClientProcessMap>()
            //.Property(e => e.RuleType)
            //.IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
            .Property(e => e.EmployeeEmailID)
            .IsUnicode(false);

            modelBuilder.Entity<UserDetail>()
            .HasMany(e => e.MailboxAccesses)
            .WithRequired(e => e.UserDetail)
            .WillCascadeOnDelete(false);

            //modelBuilder.Entity<UserDetail>()
            //.HasMany(e => e.Signatures)
            //.WithRequired(e => e.UserDetail)
            //.WillCascadeOnDelete(false);
        }

    }
    public partial class HolidayDBContext : DbContext
    {

        public HolidayDBContext() : base("name=HolidayDBConnection")
        {
        }
        public virtual DbSet<Holiday> Holidays { get; set; }
    }
}
