using CQCMS.EmailApp.Models;
using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CQCMS.Providers.DataAccess
{
    public class CaseAllData
    {
        public IDictionary<string, int> dictComplexity = new Dictionary<string, int>();
        //public static BoltDbContext gk = new BoltDbContext ();
        public async Task<CaseDetailVM> GetCaseByCaseIDAsync(string userCountry, int? CaseID)
        {
            var returnCase = new List<CaseDetailVM>() { await GetCaseByCaseIDEmptyAsync(userCountry, CaseID) };
            await PopulateCaseVirtualFieldsAsync(returnCase);
            return returnCase.FirstOrDefault();
        }
        public CaseDetailVM GetCaseByCaseID(string userCountry, int? CaseID)
        {
            var returnCase = new List<CaseDetailVM>() { GetCaseByCaseIDEmpty(userCountry, CaseID) };
            PopulateCaseVirtualFields(ref returnCase);
            return returnCase.FirstOrDefault();
        }
        public static CaseDetailVM GetCaseByCaseIDBabyCase(string userCountry, int? CaseID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                SqlParameter sqlCaseID = new SqlParameter("@ID", CaseID);
                if (CaseID == null)
                {
                    sqlCaseID.Value = DBNull.Value;
                }

                var currentCase = db.Database.SqlQuery<CaseDetailVM>("exec [db0].[getCaseByCaseID] @country, @CaseID", new SqlParameter("@country",
                    userCountry), sqlCaseID).ToList();

                var returnCase = new List<CaseDetailVM>() { currentCase.FirstOrDefault() };
                PopulateCaseVirtualFields(ref returnCase);

                return returnCase.FirstOrDefault();
            }

        }
        public bool ReclassifyCaseByCaseID(DateTime LastActedOn, string LastActedBy, int CaseID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                string execQuery = "exec [dbo].[ReclassifyCaseByCaseID] @CaseId, @LastActedon, @LastActedBy";
                db.Database.ExecuteSqlCommand(execQuery, new SqlParameter("@CaseId", CaseID), new SqlParameter("@LastActedon", LastActedOn),
                new SqlParameter("@LastActedBy", LastActedBy));
                return true;
            }

        }
        public List<CaseDetailVM> GetAllBabyCasesListSync(string userCountry, int CaseID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseID = new SqlParameter("@parsntcaseid", CaseID);
                string execQuery = "exec [dbo]. [GetAllBabyCases] @country, @parsnicassid";
                var allBabyCases = db.Database.SqlQuery<CaseDetailVM>(execQuery, new SqlParameter("@country", userCountry), sqlCaseID).ToList();
                return allBabyCases;

            }
        }
        public async Task<CaseDetailVM> GetCaseByCaseIDEmptyAsync(string userCountry, int? CaseID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);
                if (CaseID == null)
                {
                    sqlCaseID.Value = DBNull.Value;
                }
                return db.Database.SqlQuery<CaseDetailVM>("exec [dbg].[getCaseByCaseID] @country, @CaseID", new SqlParameter("@country", userCountry), sqlCaseID).FirstOrDefault();
            }
        }
        public CaseDetailVM GetCaseByCaseIDEmpty(string userCountry, int? CaseID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);
                if (CaseID == null)
                {

                    sqlCaseID.Value = DBNull.Value;
                }
                return db.Database.SqlQuery<CaseDetailVM>("exec [gyg].[getCaseByCaseID] @country, @CaseID", new SqlParameter("@country", userCountry), sqlCaseID).FirstOrDefault();
            }
        }
        public static string GenerateCaseldIdentifier(string userCountry, int? CaseID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);
                if (CaseID == null)
                {
                    sqlCaseID.Value = DBNull.Value;
                }
                var result = db.Database.SqlQuery<string>("exec [dbo].[GenerateCaseididentifier] @Caseld, @Country", sqlCaseID, new SqlParameter("@country", userCountry)).ToList();
                return result.FirstOrDefault();
            }
        }
        public static string GenerateBabyCaseIdidentifier(string userCountry, int? CaseID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);
                if (CaseID == null)
                {
                    sqlCaseID.Value = DBNull.Value;
                }
                var result = db.Database.SqlQuery<string>("exec [dbo].[GenerateBabyCaseldIdentitier] @Caseld,@Country", sqlCaseID, new SqlParameter("@country", userCountry)).ToList();
                return result.FirstOrDefault();

            }
        }
        public async Task<CaseDetailVM> GetCaseByLastEmailId(string userCountry, int? lastemailid)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                CaseDetailVM currentCase = await GetCaseByLastEmailIdEmpty(userCountry, lastemailid);
                var returnCase = new List<CaseDetailVM>() { currentCase };
                await PopulateCaseVirtualFieldsAsync(returnCase);
                return returnCase.FirstOrDefault();
            }
        }
        private async Task<CaseDetailVM> GetCaseByLastEmailIdEmpty(string userCountry, int? lastEmailid)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlEmailld = new SqlParameter("@Lastimailia", lastEmailid);
                if (lastEmailid == null)
                {
                    sqlEmailld.Value = DBNull.Value;
                }
                return db.Database.SqlQuery<CaseDetailVM>("exec [dbo].[getCaseByLastEmailId] @country, @Lastimailid", new SqlParameter("@country", userCountry), sqlEmailld).FirstOrDefault();

            }
        }
        //public async Task<CaseDataVM> GetCaseDataByCaseID(string userCountry, int? CaseID)
        //{

        //    using (CQCMSDbContext db = new CQCMSDbContext())
        //    {

        //        SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);

        //        if (CaseID == null)
        //        {
        //            sqlCaseID.Value = DBNull.Value;
        //        }
        //        var currentCase = await db.Database.SqlQuery<CaseDataVM>("exec [dbo].[getCaseDataByCaseID] @country, @CaseID", new SqlParameter("@country", userCountry), sqlCaseID).SingleOrDefaultAsync();
        //        return currentCase;

        //    }
        //}
        public async Task PopulateCaseVirtualFieldsAsync(List<CaseDetailVM> caseDetails)
        {
            CategoryData catData = new CategoryData();
            EmailData emailData = new EmailData();
            MailboxData mailboxData = new MailboxData();
            foreach (CaseDetailVM caseDetail in caseDetails)
            {
                if (caseDetail != null)
                {
                    caseDetail.CaseStatusLookup = await GetCaseStatusLookUpByID(caseDetail.Country, caseDetail.CaseStatusID);
                    caseDetail.Category = await catData.GetCategorybyCategoryIDAsync(caseDetail.Country, caseDetail.CategoryID);
                    caseDetail.Mailbox = mailboxData.GetMailboxbyID(caseDetail.Country, caseDetail.MailboxID);
                    //caseDetail.fmails = await EmailData.GetEmailByCaseIDForVirtualAsync(caseDetail.country, caseDetail.CaseID)
                    caseDetail.EmailAttachments = await emailData.GetEmailAttachemntByEmailIdAsync(caseDetail.Country, caseDetail.LastEmailID);
                    caseDetail.SubCategory = await catData.GetSubCategorybySubCategoryIDAsync(caseDetail.Country, caseDetail.SubCategoryID);
                }
            }
        }



        public static void PopulateCaseVirtualFields(ref List<CaseDetailVM> caseDetails)

        {
            foreach (CaseDetailVM caseDetail in caseDetails)
            {
                if (caseDetail != null && caseDetail.CaseID != 0)
                {
                    caseDetail.CaseStatusLookup = GetCaseStatusLookUpByIDBabyCase(caseDetail.Country, caseDetail.CaseStatusID);
                    caseDetail.Category = CategoryData.GetCategorybyCategoryIDBabyCase(caseDetail.Country, caseDetail.CategoryID);
                    caseDetail.Mailbox = new MailboxData().GetMailboxbyID(caseDetail.Country, caseDetail.MailboxID);
                    caseDetail.Emails = EmailData.GetEmailByCaseIDForVirtualBabyCase(caseDetail.Country, caseDetail.CaseID);
                    caseDetail.EmailAttachments = EmailData.GetEmailAttachemntByEmailIdBabyCase(caseDetail.Country, caseDetail.LastEmailID);
                    caseDetail.SubCategory = CategoryData.GetSubCategorybySubCategoryIDBabyCase(caseDetail.Country, caseDetail.SubCategoryID);
                }
            }
        }
        //public static void PopulateCaseVirtualFields(ref List<CaseDetailVM> caseDetails)
        //{
        //    foreach (CaseDetailVM caseDetail in caseDetails)
        //    {
        //        if (caseDetail != null && caseDetail.CaseID != 0)
        //        {

                    //            caseDetail.CaseStatusLookup = GetCaseStatusLockUpByIDBabyCase(caseDetail.Country, caseDetail.CaseStatusID);
                    //            caseDetail.Category = CategoryData.GetCategorybyCategoryIDBabyCase(caseDetail.Country, caseDetail.CategoryID);
                    //            caseDetail.Mailbox = new MailboxData().GetMailboxbyID(caseDetail.Country, caseDetail.MailboxID);

                    //            caseDetail.Emails = EmailData.GetEmailByCaseIDForVirtualBabyCase(caseDetail.Country, caseDetail.CaseID);
                    //            caseDetail.EmailAttachments = EmailData.GetEmailattachemntByfmailIdBabyCase(caseDetail.Country, caseDetail.LastEmailID);
                    //            caseDetail.SubCategory = CategoryData.GetSubCategorybySubCategoryIDBabyCase(caseDetail.Country, caseDetail.SubCategoryID);
                    //        }
                    //    }
                    //}
        public async Task<CaseStatusLookup> GetCaseStatusLookUpByID(string userCountry, int casestatusid)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                if (HttpContext.Current == null)
                {
                    return await db.Database.SqlQuery<CaseStatusLookup>("exec [gg]. [GetCaseStatushookUpByID] @CaseStatusTD, @country",
                    new SqlParameter("@CaseStatusID", casestatusid), new SqlParameter("@country", userCountry)).SingleOrDefaultAsync();
                }
                else
                    return GetAllCaseStatusLookup().FirstOrDefault(l => l.CaseStatusID == casestatusid);
            }
        }
        public static CaseStatusLookup GetCaseStatusLookUpByIDBabyCase(string userCountry, int casestatusid)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                if (HttpContext.Current == null)
                {

                    var caseStatusLookup = db.Database.SqlQuery<CaseStatusLookup>("exec [gag] - [GetCaseStatushookUpByID] @CaseStatusID, @country",
                    new SqlParameter("@CaseStatusID", casestatusid), new SqlParameter("@country", userCountry)).FirstOrDefault();
                    return caseStatusLookup;

                }
                else
                    return GetAllCaseStatusLookup().FirstOrDefault(l => l.CaseStatusID == casestatusid);
            }
        }
        public static List<CaseStatusLookup> GetAllCaseStatusLookup()
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                List<CaseStatusLookup> caseStatus = new List<CaseStatusLookup>();
                if (HttpContext.Current != null && HttpContext.Current.Cache["CaseStatusLookup"] != null)
                    return (List<CaseStatusLookup>)HttpContext.Current.Cache["CaseStatusLookup"];
                if (HttpContext.Current == null || HttpContext.Current.Cache["CaseStatusLookup"] == null)
                {
                    caseStatus = db.Database.SqlQuery<CaseStatusLookup>("exec [dbo].[GetAllCaseStatusLookUp]").ToList();
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Cache.Add("CaseStatusLockup", caseStatus, null, DateTime.Now.AddMinutes(60),
                        System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                    }
                }
                return caseStatus;
            }
        }
    }
}
