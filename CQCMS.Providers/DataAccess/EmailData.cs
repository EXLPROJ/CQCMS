using CQCMS.EmailApp.Models;
using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
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

    }
}
