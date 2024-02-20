using CQCMS.EmailApp.Models;
using CQCMS.Entities;
using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

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
        public async Task<CaseDetailVM> GetCaseByCaseldentifier(string userCountry, string caseIdIdentifier)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                string execQuery = "exec [dbo].[GetCaseByCaseldentifier] @country, @caseIdIdentifier";
                if (caseIdIdentifier == null || caseIdIdentifier == "")
                {
                    return null;
                }

                return db.Database.SqlQuery<CaseDetailVM>(execQuery, new SqlParameter("@caseIdIdentifier", caseIdIdentifier), new SqlParameter("@country", userCountry)).FirstOrDefault();
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
                return db.Database.SqlQuery<CaseDetailVM>("exec [dbg].[getCaseByCaseID] @country, @CaseID", new SqlParameter("@country",
                userCountry), sqlCaseID).FirstOrDefault();
            }
        }




        public void InsertIntoCaseAuditTrailTable(string Comment, int CaseId, string ActedBy, string Action)

        {
            try
            {
                using (CQCMSDbContext db = new CQCMSDbContext())
                {
                    if (ActedBy == null)
                    {
                        ActedBy = Environment.UserName;

                    }

                    SqlParameter sqlActedOn = new SqlParameter("@ActedOn", DateTime.Now);

                    string execQuery = "exec [dbo].[InsertIntoCaseAuditTrailTable] @CaseID,@Action, @ActedBy, @ActedOn,@Comment";


                    db.Database.ExecuteSqlCommand(execQuery, new SqlParameter("@CaseID", CaseId), new SqlParameter("@Action", Action), new SqlParameter("@ActedBy", ActedBy), sqlActedOn, new SqlParameter("@Comment", Comment));

                }

            }

            catch (Exception ex) { }
        }

        public async Task<bool> UpdateSubmittedAttributeByCaseId(int caseld, string customattribute)

        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                try
                {
                    return await db.Database.SqlQuery<bool>("exec [dbo] . [UpdateSubmittedAttributeByCaseld] @Caseld,@CustomAttribute", new SqlParameter("@Caseld", caseld), new SqlParameter("@CustomAttribute", customattribute)).SingleOrDefaultAsync();
                }
                catch (Exception ex) { return false; }
            }
        }


        public async Task<CaseDetailVM> GetCaseByCaseIDForVirtual(string userCountry, int? CaseID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);
                if (CaseID == null)
                {
                    sqlCaseID.Value = DBNull.Value;

                }


                return await db.Database.SqlQuery<CaseDetailVM>("exec [dbo]. [getCaseByCaseID] @country, @CaseID", new SqlParameter("@country", userCountry), sqlCaseID).SingleOrDefaultAsync();
                //return currentCase;
            }
        }



        public async Task<CaseDetailVM> UpdateCaseUIDataByCaseID(string userCountry, CaseDetailUI updateCase)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                CaseDetailVM updatedCase = new CaseDetailVM();
                var param = HelperFunctions.CreateParameterListfromModelWithoutIdout(updateCase);
                string execQuery = "exec [dbo].[updateCaseUIDataByCaseID] @CaseID, @MailboxID, @EchoCaseNumber,@CurrentlyAssignedTo, @PreviouslyAssignedTo," +
                "@IsAssigned, @AssignedTime, @CaseStatusID, @CreatedOn, @CreatedBy, @LastActedOn,@lastActedBy, @Comments, @AdditionalClientInfo, @IsCaseClosed," +
                "@ClosedOn, @ClosedBy, @IsComplaint, @IsPhoneCall, @CategoryID, @SubCategoryID, @CIN,@ClientName, @AccountNumber, @BusinessSegment, @BusinessLineCode," +
                "@PendingStatus, @CaseAdditionalDetail, @lastEmailID, @FirstEmailID, @IsFlagged,@Priority, @FollowpDate, @SLADueDate, @TouchDueDate, @CaseReOpenedOn," +
                "@ComplaintOn, @ComplaintProductorService, @ComplaintRootCause, @CompleintAreaofOccurence,@ComplaintOutcome, @ImpactToClient, @ComplaintErrorCode," +
                "@ComplaintContactChannel, @IsFeeReversal, @FeeReversalAmount, @FeeReversalReason,@CaseAssignAttempts, @DoesPartialSubjectMatch, @IsCaseComplaintIntegrated," +
                "@latchedPartialCases, @country, @KeepWithMe, @NoOFQueries ,@EscalationRootCauseID,@EscalationOriginatorID, @EscalationOriginatorName," +
                "@IsMLClassified, @MLClassifiedCategoryID, @MLClassifiedSubCategoryID,pReClassificationTriggeredBy ,@ForceReclassificationBy ,@MLReclassificationCounter";
                updatedCase = await db.Database.SqlQuery<CaseDetailVM>(execQuery, param.ToArray()).SingleOrDefaultAsync();
                var returnUpdatedCase = new List<CaseDetailVM>() { updatedCase };
                await PopulateCaseVirtualFieldsAsync(returnUpdatedCase);
                return returnUpdatedCase.FirstOrDefault();
            }
        }

        public void UpdateFiledCasesToAssigned()
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                try
                {
                    db.Database.ExecuteSqlCommand("exec [dbo].[UpdateFiledCasesToAssigned]");
                }
                catch (Exception ex) { }
            }
        }

        public static string GenerateCaseIdIdentifier(string usercountry, int? CaseID)

        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);
                if (CaseID == null)
                {

                    sqlCaseID.Value = DBNull.Value;
                }

                var result = db.Database.SqlQuery<string>("exec [dbo] .[GenerateCaseIdIdentifier] @CaseId,@Country", sqlCaseID, new SqlParameter("@country", userCountry)).ToList();
                return result.FirstOrDefault();


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
                return db.Database.SqlQuery<CaseDetailVM>("exec [gyg].[getCaseByCaseID] @country, @CaseID", new SqlParameter("@country", userCountry), sqlCaseID).FirstorDefault();
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
                return await db.Database.SqlQuery<CaseDetailVM>("exec [dbo].[getCaseByLastEmailId] @country, @Lastimailid", new SqlParameter("@country", userCountry), sqlEmailld).FirstOrDefault();

            }
        }
        public async Task<CaseDataVM> GetCaseDataByCaseID(string userCountry, int? CaseID)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);

                if (CaseID == null)
                {
                    sqlCaseID.Value = DBNull.Value;
                }
                var currentCase = await db.Database.SqlQuery<CaseDataVM>("exec [dbo].[getCaseDataByCaseID] @country, @CaseID", new SqlParameter("@country", userCountry), sqlCaseID).SingleOrDefaultAsync();
                return currentCase;

            }
        }
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
                    caseDetail.EmailAttachments = await emailData.GetemailattachemntByEmailIdAsync(caseDetail.Country, caseDetail.LastEmailID);
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

                    caseDetail.CaseStatusLookup = GetCaseStatusLockUpByIDBabyCase(caseDetail.Country, caseDetail.CaseStatusID);
                    caseDetail.Category = CategoryData.GetCategorybyCategoryIDBabyCase(caseDetail.Country, caseDetail.CategoryID);
                    caseDetail.Mailbox = new MailboxData().GetMailboxbyID(caseDetail.Country, caseDetail.MailboxID);

                    caseDetail.Emails = EmailData.GetEmailByCaseIDForVirtualBabyCase(caseDetail.Country, caseDetail.CaseID);
                    caseDetail.EmailAttachments = EmailData.GetEmailattachemntByfmailIdBabyCase(caseDetail.Country, caseDetail.LastEmailID);
                    caseDetail.SubCategory = CategoryData.GetSubCategorybySubCategoryIDBabyCase(caseDetail.Country, caseDetail.SubCategoryID);
                }
            }
        }
        public async Task<CaseStatusLookup> GetCaseStatusLookUpByID(string userCountry, int casestatusid)

        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                if (HttpContext.Current == null)
                {
                    return await db.Database.SqlQuery<CaseStatusLookup>("exec [gg]. [GetCaseStatushookUpByID] @CaseStatusTD, @country",
                    new SqlParameter("@CaseStatusID", casestatusid), new SqlParameter("@country", userCountry)).SingleOrDefaultasync();
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
                    caseStatus = db.Database.Sqlquery<CaseStatusLookup>("exec [dbo].[GetAllCaseStatusLookUp]").ToList();
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Cache.Add("CaseStatusLockup", caseStatus, null, DateTime.Now.AddMinutes(60),
                        System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                    }
                }
                return caseStatus;
            }
        }




        public async Task<CaseDetailVM> UpdateSLADueDatebyID(string userCountry, DateTime lastActedOn, string lastActedBy, int caseId, DateTime? SLADueDate)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                SqlParameter sqlSLADueDate = new SqlParameter("@SLADueDate", SLADueDate);

                if (SLADueDate == null)
                {

                    sqlSLADueDate.Value = DBNull.Value;
                }

                if (lastActedBy == null && lastActedBy == "")
                {
                    lastActedBy = Environment.UserName;
                }

                CaseDetailVM updatedCase = new CaseDetailVM();
                string execQuery = "exec[dbo].[UpdateSLADueDatebyID] @CaseID, @lastActedOn, @LastActedBy,@country,@SLADueDate";
                updatedCase = await db.Database.SqlQuery<CaseDetailVM>(execQuery, new SqlParameter("@CaseID", caseId), new SqlParameter("@LastActedOn", lastActedOn), new SqlParameter("@LastActedBy", lastActedBy), new SqlParameter("@country", userCountry), sqlSLADueDate).SingleOrDefaultAsync();



                var returnUpdatedCase = new List<CaseDetailVM>() { updatedCase };
                await PopulateCaseVirtualFieldsAsync(returnUpdatedCase);
                return returnUpdatedCase.FirstOrDefault();


            }
        }





        public async Task<CaseDetailVM> UpdateTouchDueDatebyID(string userCountry, DateTime lastActedOn, string lastActedBy, int CaseID, DateTime? TouchDueDate)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                SqlParameter sqlTouchDate = new SqlParameter("@TouchDueDate", TouchDueDate);

                if (TouchDueDate == null)
                {
                    sqlTouchDate.Value = DBNull.Value;
                }

                CaseDetailVM updatedCase = new CaseDetailVM();
                string execQuery = "exec [dbo].[UpdateTouchDueDatebyID] @CaseID, @lastActedOn, @lastActedBy,@country ,@TouchDueDate";
                updatedCase = await db.Database.SqlQuery<CaseDetailVM>(execQuery, new SqlParameter("@CaseID", CaseID), new SqlParameter("@LastActedOn", lastActedOn), new SqlParameter("@LastActedBy", lastActedBy), new SqlParameter("@country", userCountry), sqlTouchDate).SingleOrDefaultAsync();
                var returnUpdatedCase = new List<CaseDetailVM>() { updatedCase };
                await PopulateCaseVirtualFieldsAsync(returnUpdatedCase);
                return returnUpdatedCase.FirstOrDefault();

            }
        }

        public async Task<DateTime?> CalcTouchDueDate(int CaseID = 0)

        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                var data = await db.Database.SqlQuery<DateTime?>("select dbo.CalcTouchDueDate('" + CaseID + "')").SingleOrDefaultAsync();
                return data;
            }
        }

        public bool CheckIfCaseHasRollover(int CaseID)
        {
            try
            {
                using (CQCMSDbContext db = new CQCMSDbContext())
                {
                    return db.Database.SqlQuery<bool>("select dbo.CheckIfCaseHasRollOver @CaseId", new SqlParameter("@CaseID", CaseID)).SingleOrDefault();

                }
            }
            catch (Exception ex) { return false; }
        }

        public async Task<CaseDetailVM> UpdateCaseAssignedTime(string userCountry, CaseAssignedTime caseAssignedTime)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                CaseDetailVM updatedCase = new CaseDetailVM();
                string execQuery = "exec [dbo]. [UpdateCaseAssignedTime] @CaseID, @PreviouslyAssignedTo, @CurrentlyAssignedTo,@AssignedTime, @LastActedOn, @lastActedBy, @IsAssigned, @country";
                var param = HelperFunctions.CreateParameterListfromModelWithoutIdout(caseAssignedTime);
                updatedCase = await db.Database.SqlQuery<CaseDetailVM>(execQuery, param.ToArray()).SingleOrDefaultAsync();
                var returnUpdatedCase = new List<CaseDetailVM>() { updatedCase };
                await PopulateCaseVirtualFieldsAsync(returnUpdatedCase);
                return returnUpdatedCase.FirstOrDefault();
            }
        }


        public async Task<CaseDetailVM> UpdateCaseAssignAttempts(string userCountry, int caseID, DateTime? lastActedOn, string lastActedBy, int assignAttemptValue)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                CaseDetailVM updatedCase = new CaseDetailVM();
                string execQuery = "exec [dbo]. [UpdateCaseAssignAttempts] @CaseID, @LastActedOn, @LastActedBy, @CaseAssignAttempts, @country";
                updatedCase = await db.Database.SqlQuery<CaseDetailVM>(execQuery, new SqlParameter("@CaseID", caseID), new SqlParameter("@LastActed0n", lastActedOn), new SqlParameter("@LastActedBy", lastActedBy), new SqlParameter("@CaseAssignAttempts", assignAttemptValue), new SqlParameter("@country", userCountry)).FirstOrDefaultAsync();
                var returnUpdatedCase = new List<CaseDetailVM>() { updatedCase };
                await PopulateCaseVirtualFieldsAsync(returnUpdatedCase);
                return updatedCase;
            }
        }

        public async Task<CaseDetailVM> UpdateCaseRelease(string userCountry, CaseRelease caseRelease)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                CaseDetailVM updatedCase = new CaseDetailVM();
                var param = HelperFunctions.CreateParameterListfromModelWithoutIdout(caseRelease);
                string execQuery = "exec [dbo].[UpdateCaseRelease] @CaseID, @LastActedOn, @LastActedBy, @country, @IsAssigned,PreviouslyAssignedTo, @CurrentlyAssignedTo,@CaseAssignAttempts, @CaseEscalatedManager, @AssignedTime";
                updatedCase = await db.Database.SqlQuery<CaseDetailVM>(execQuery, param.ToArray()).SingleOrDefaultAsync();
                var returnUpdatedCase = new List<CaseDetailVM>() { updatedCase };
                await PopulateCaseVirtualFieldsAsync(returnUpdatedCase);
                return returnUpdatedCase.FirstOrDefault();
            }
        }


        public async Task<int> UpdateCaseNewEmailCount(string userCountry, int? CaseID)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())

            {
                SqlParameter sqlCaseID = new SqlParameter("@CaseID", CaseID);
                if (CaseID == null)
                {

                    sqlCaseID.Value = DBNull.Value;
                }

                return await db.Database.ExecuteSqlCommandAsync("exec [dbo] . [UpdateCaseNewEmailCount] @country, @CaseID", new SqlParameter("@country", userCountry), sqlCaseID);

            }
        }

    }
}
