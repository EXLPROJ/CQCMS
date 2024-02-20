using CQCMS.EmailApp.Models;
using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CQCMS.Providers.DataAccess
{
    public class MailboxData
    {

        public List<MailboxVM> GetAllMailbox(string userCountry)
        {
            List<MailboxVM> mailbox = new List<MailboxVM>();
            if (HttpContext.Current != null && HttpContext.Current.Cache["AllMailboxes_" + userCountry] != null)
                return (List<MailboxVM>)HttpContext.Current.Cache["AllMailboxes_" + userCountry];
            if (HttpContext.Current == null || HttpContext.Current.Cache["AllMailboxes_" + userCountry] == null)
            {
                {
                    SqlParameter country = new SqlParameter("@Country", userCountry);
                    if (userCountry == null)
                    {
                        country.Value = DBNull.Value;
                    }
                    using (CQCMSDbContext db = new CQCMSDbContext())
                        mailbox = db.Database.SqlQuery<MailboxVM>("exec [dbo].[getAllActiveMailboxByCountry] @country", country).ToList();
                    SetMailboxDataToCache(userCountry, mailbox);
                }
               
            }
            return mailbox;
        }


        private static void SetMailboxDataToCache(string userCountry, List<MailboxVM> mailboxes)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Cache.Add("AllMailboxes_" + userCountry, mailboxes, null, DateTime.Now.AddMinutes(60),System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
        }

        public MailboxVM GetMailboxbyID(string userCountry, int? mailboxID)
        {
            if (mailboxID == null)
                return null;

            var allMailbox = GetAllMailbox(userCountry);
            return allMailbox.FirstOrDefault(mb => mb.MailboxID == mailboxID.Value);
        }
    }
}
