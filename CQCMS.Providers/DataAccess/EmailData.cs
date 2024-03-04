using CQCMS.Entities;
using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace CQCMS.Providers.DataAccess
{
    public class EmailData
    {
        public static List<PartialCaseMatch> FindPartiallyMatchingCases(string Emailsubject, DateTime? SentOn, DateTime? ReceivedOn, string TextBody, string ToReceipients, string CCReceipients, string EmailFrom, int? MailboxId, string MailboxCountry)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())

            {

                SqlParameter sqlMailboxID = new SqlParameter("@mailboxId", MailboxId);
                if (MailboxId == null)
                {
                    sqlMailboxID.Value = DBNull.Value;
                }
                SqlParameter sqlSenton = new SqlParameter("@SentOn", SentOn.Value.ToString("dd-MMM-yyyy HH:mm:ss") ?? ReceivedOn.Value.ToString("dd-MMM-yyyy HH:mm:ss"));

                var matchedCases = db.Database.SqlQuery<PartialCaseMatch>("exec [dbo].[FindPartiallyMatchingCases] @country, @mailBoxId,@EmailSubject, @TextBody, @SentOn, @senderEmail, @emailTo, @EmailCc", new SqlParameter("@country", MailboxCountry),
                sqlMailboxID, new SqlParameter("@EmailSubject", Emailsubject), new SqlParameter("@TextBody", TextBody), sqlSenton,
                new SqlParameter("@SenderEmail", EmailFrom),
                new SqlParameter("@EmailTo", ToReceipients), new SqlParameter("@EmailCc", CCReceipients)).ToList();

                return matchedCases;
            }
        }

        public static int IsEmailPresentWithHash(string CaseId, string EmailHash, String SentOn, string Country)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseId = new SqlParameter("@caseid", CaseId);
                if (CaseId == null)
                {
                    sqlCaseId.Value = DBNull.Value;
                }

                var matchedEmails = db.Database.SqlQuery<int>("exec [dbo].[IsEmailPresentWithHash] @CaseId, @EmailHash, @Country,@SentOn", sqlCaseId,
                new SqlParameter("@EmailHash", EmailHash), new SqlParameter("@Country", Country), new SqlParameter("@SentOn", SentOn)).
                FirstOrDefault();
                return matchedEmails;
            }
        }

        public async Task<List<EmailAttachmentVM>> GetEmailAttachemntByEmailIdAsync(string userCountry, int? EmailId, bool SkipInlineAttachments = false)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                SqlParameter sqlEmailId = new SqlParameter("@EmailId", EmailId);
                if (EmailId == null)
                {
                    sqlEmailId.Value = DBNull.Value;
                }
                var allAttachments = await db.Database.SqlQuery<EmailAttachmentVM>("exec [dbo].[GetEmailattachemntByEmailId] @Country, @EmailId",
                new SqlParameter("@Country", userCountry), sqlEmailId).ToListAsync();
                if (SkipInlineAttachments)
                    return allAttachments.Where(a => a.IsInline == false).ToList();
                else
                    return allAttachments;

            }
        }

        public String CleanEmailSubject(string EmailSubject = "")
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                var data = db.Database.SqlQuery<String>("select dbo.CleanEmailSubject('" + EmailSubject + "'").SingleOrDefault();
                return data;
            }
        }

        public async Task<String> CleanEmailSubjectAsync(string Emailsubject = "")
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                var data = await db.Database.SqlQuery<String>("select dbo.CleanEmailsubject('" + Emailsubject + "'").SingleOrDefaultAsync();
                return data;
            }
        }

        public static List<EmailAttachmentVM> GetEmailAttachementByEmailIdBabyCase(string userCountry, int? EmailId)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlEmailId = new SqlParameter("@EmailId", EmailId);
                if (EmailId == null)
                {
                    sqlEmailId.Value = DBNull.Value;
                }
                var attachments = db.Database.SqlQuery<EmailAttachmentVM>("exec [dbo].[GetEmailAttachemntByEmailId] @Country, @EmailId",
                new SqlParameter("@Country", userCountry), sqlEmailId).ToList();
                return attachments;
            }
        }

        public async Task<List<EmailAttachmentVM>> GetEmailAttachementByEmailId(string userCountry, int? Emailld, bool SkipInlineAttachments = false)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlEmailId = new SqlParameter("@Emailld", Emailld);

                if (Emailld == null)
                {

                    sqlEmailId.Value = DBNull.Value;
                }

                var allAttachments = await db.Database.SqlQuery<EmailAttachmentVM>("exec [dbo].[GetEmailAttachemntByEmailId] @Country, @EmailId", new SqlParameter("@Country", userCountry), sqlEmailId).ToListAsync();
                if (SkipInlineAttachments)
                    return allAttachments.Where(a => a.IsInline == false).ToList();
                else
                    return allAttachments;
            }
        }

        public static List<EmailVM> GetEmailByCaseIDForVirtualBabyCase(string userCountry, int? caseid)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseId = new SqlParameter("@caseid", caseid);
                if (caseid == null)
                {
                    sqlCaseId.Value = DBNull.Value;
                }
                var emails = db.Database.SqlQuery<EmailVM>("exec [dbo].[GetEmailByCaseID] @country, @caseid",
                new SqlParameter("@country", userCountry), sqlCaseId).ToList();
                return emails;
            }

        }

        public async Task<EmailAttachmentVM> InsertIntoEmailAttachmentTable(EmailAttachmentInsert emailAttachmentUI)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                var param = HelperFunctions.CreateParameterListfromModelWithoutIdout(emailAttachmentUI);
                param.Add(new SqlParameter("Id", DbType.Int32)
                {

                    Direction = ParameterDirection.Output
                });
                string execQuery = "exec [dbo].[InsertIntoEmailAttachmentTableWithIsInline] @EmailFileID, @EmailID, @CaseID,@EmailFileName, @EmailOriginalFileName, @EmailFilePath, @IsActive, @Createdon, @Country, @LastActedBy, @LastActedOn,@IsInline, @Id out";

                return await db.Database.SqlQuery<EmailAttachmentVM>(execQuery, param.ToArray()).FirstOrDefaultAsync();
            }
        }

        public async Task<EmailVM> GetEmailbyEmailIdAsync(string userCountry, int? EmailId)

        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                SqlParameter sqlEmailld = new SqlParameter("@Emailld", EmailId);

                if (EmailId == null)
                {

                    sqlEmailld.Value = DBNull.Value;
                }

                var email = await db.Database.SqlQuery<EmailVM>("exec [dbo].[GetEmailbyEmailId] @Country,@EmailId", new SqlParameter("@Country", userCountry), sqlEmailld).FirstOrDefaultAsync();


                await PopulateEmailVirtualFields(userCountry, new List<EmailVM> { email });

                return email;
            }
        }

        public async Task PopulateEmailVirtualFields(string userCountry, List<EmailVM> emails)
        {
            var caseData = new CaseAllData();
            var mailboxData = new MailboxData();

            foreach (var email in emails)

            {
                if (email != null)
                {

                    email.CaseDetail = await caseData.GetCaseByCaseIDForVirtual(userCountry, email.CaseID);
                    email.EmailType = await GetEmailTypeByEmailTypeIdAsync(userCountry, email.EmailTypeID);
                    email.Mailbox = mailboxData.GetMailboxbyID(userCountry, email.MailboxID);
                    email.EmailAttachments = await GetEmailAttachemntByEmailIdAsync(userCountry, email.EmailID);
                    if (email.CaseDetail != null)
                    {

                        email.CaseStatusLookup = await caseData.GetCaseStatusLookUpByID(userCountry, email.CaseDetail.CaseStatusID);

                    }
                }
            }
        }

        public async Task<EmailType> GetEmailTypeByEmailTypeIdAsync(string userCountry, int? EmailTypeID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlEmailTypeID = new SqlParameter("@EmailTypeID", EmailTypeID);

                if (EmailTypeID == null)
                {

                    sqlEmailTypeID.Value = DBNull.Value;
                }

                var emailType = await db.Database.SqlQuery<EmailTypeVM>("exec [dbo].[GetEmailTypeByEmailTypeId] @country,@EmailTypeID", new SqlParameter("@country", userCountry), sqlEmailTypeID).FirstOrDefaultAsync();

                EmailType emailTypeReturn = new EmailType();

                ConvertEmailTypeVMToEmailType(emailType, ref emailTypeReturn);

                return emailTypeReturn;
            }
        }

        public void ConvertEmailTypeVMToEmailType(EmailTypeVM emailTypeVM, ref EmailType emailType)
        {
            emailType.EmailTypeID = emailTypeVM.EmailTypeID;
            emailType.EmailType1 = emailTypeVM.EmailType1;
            emailType.IsActive = emailTypeVM.IsActive;
            emailType.LastActedOn = emailTypeVM.LastActedon;
            emailType.LastActedBy = emailTypeVM.LastActedBy;
        }
        public void CleanupEmailTraces(int? EmailId)
        {
            try
            {
                using (CQCMSDbContext db = new CQCMSDbContext())
                {
                    SqlParameter sqlEmailId = new SqlParameter("@EmailId", EmailId);
                    if (EmailId == null || EmailId == 0)
                    {
                        return;
                    }
                    string execQuery = "exec [dbo].[CleanupEmailTraces] @EmailId";
                    db.Database.ExecuteSqlCommand(execQuery, sqlEmailId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
