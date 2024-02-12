using CQCMS.EmailApp.Models;
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
        public static List<PartialCaseMatch> FindPartiallyMatchingCases(string Emailsubject, DateTime? SentOn, DateTime? ReceivedOn, string TextBody,
        string ToReceipients, string CCReceipients, string EmailFrom, int? MailboxId, string MailboxCountry)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())

            {

                SqlParameter sqlMailboxID = new SqlParameter("@mailboxId", MailboxId);
                if (MailboxId == null)
                {
                    sqlMailboxID.Value = DBNull.Value;
                }
                SqlParameter sqlSenton = new SqlParameter("@SentOn", SentOn.Value.ToString("dd-MMM-yyyy HH:mm:ss") ?? ReceivedOn.Value.
                ToString("@g-MMM-yyyy HH:mm:g§"));

                var matchedCases = db.Database.SqlQuery<PartialCaseMatch>("exec [dbo]. [FindPartiallyMatchingCases] @country, @mailboxid,@Emailsubject, @TextBody, @SentOn, @senderEmail, @emailto, @EmailCc", new SqlParameter("@country", MailboxCountry),
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

                var matchedEmails = db.Database.SqlQuery<int>("exec [gpg]. [IsEmailPresentWithHash] @CaseId, @EmailHash, @Country,@SentOn", sqlCaseId,
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
                var data = db.Database.SqlQuery<String>("select dbo.CleanEmailsubject('" + EmailSubject + "'").SingleOrDefault();
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
        public static List<EmailAttachmentVM> GetEmailAttachemntByEmailIdBabyCase(string userCountry, int? EmailId)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlEmailId = new SqlParameter("@Emailla", EmailId);
                if (EmailId == null)
                {
                    sqlEmailId.Value = DBNull.Value;
                }
                var attachments = db.Database.SqlQuery<EmailAttachmentVM>("exec [dbo].[GetEmailAttachemntByFmailId] @Country, @EmailId",
                new SqlParameter("@Country", userCountry), sqlEmailId).ToList();
                return attachments;
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
                string execQuery = "exec [dbo].[InsertIntoEmailAttachmentTableWithIsInline] @EmailFileID, @EmailID, @CaseID,@EmailFileName, @EmailoriginalFileName, @EmailFilePath, @IsActive, @Createdon, @Country, @LastActedBy, @LastActedon,@IsInline, @Id out";

                return await db.Database.SqlQuery<EmailAttachmentVM>(execQuery, param.ToArray()).FirstOrDefaultAsync();
            }
        }
    }
}
