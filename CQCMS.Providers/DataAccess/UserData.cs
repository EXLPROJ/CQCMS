using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CQCMS.EmailApp.Models;
using CQCMS.Entities.Models;
using NLog;

namespace CQCMS.Providers.DataAccess
{
    public class UserData
    {
        Logger logger = LogManager.GetLogger("EmailEchoTransformation");



        public UserDetailVM GetUserInfoByEmployeeIDCountry(string userCountry, string Employeeld)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                SqlParameter sqlEmployeeId = new SqlParameter("@EmployeeId", Employeeld);

                if (Employeeld == null)
                {

                    sqlEmployeeId.Value = DBNull.Value;
                }

                return db.Database.SqlQuery<UserDetailVM>("exec [dbo]. [GetUserInfoByEmployeeIDCountry] @Employeeld, @country", sqlEmployeeId, new SqlParameter("@country", userCountry)).FirstOrDefault();

            }
        }





        public void CapacityUpdateForAllUsers()
        {
            try
            {
                using (CQCMSDbContext db = new CQCMSDbContext())
                    db.Database.ExecuteSqlCommand("exec [dbo] .[SetAllUsersCapacityAndUtilization]");
                //db.Database.SqlQuery<UserDetailVM>("exec [dbo] . [SetAl1UsersCapacityAndUtilization]").ToList()5

            }


            catch (Exception ex)

            {
                logger.Error("Exception Message in CapacityUpdateForAllUsers()");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);

            }
        }

        public UserDetailVM GetUserDetailsByID(int? userID, string userCountry)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())

            {
                return db.Database.SqlQuery<UserDetailVM>("exec [dbo].[getUserInfoByID] @ID, @country", new SqlParameter("@ID", userID), new SqlParameter("@country", userCountry)).FirstOrDefault();

            }
        }

        public UserDetailVM GetUserbyIDFirstOrDefault(string Country, int? userID)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())

            {
                return db.Database.SqlQuery<UserDetailVM>("exec [dbo].[getUserInfoByID] @ID, @country", new SqlParameter("@ID", userID), new SqlParameter("@country", Country)).FirstOrDefault();

            }
        }
        public int FindEligibleUserBasedOnHashTagIds(int caseID)
        {
            try
            {
                using (CQCMSDbContext db = new CQCMSDbContext())
                    return db.Database.SqlQuery<int>("exec [dbo].[FindUserByHashtagsIds] @ID, @country", new SqlParameter("@CaseId", caseID)).FirstOrDefault();


            }
            catch (Exception ex)
            {
                return 0;
            }

        }


        public async Task<List<UserDetailVM>> GetActiveUserMappingCategoryWiseDataAsync(string userCountry, int? categoryid, int? subcategoryid, bool isactive, bool isaway)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                SqlParameter sqlcategoryid = new SqlParameter("@categoryid", categoryid);
                SqlParameter sqlsubcategoryid = new SqlParameter("@subcategoryid", subcategoryid);

                if (categoryid == null)

                {
                    sqlcategoryid.Value = DBNull.Value;
                }
                if (subcategoryid == null)
                {

                    sqlsubcategoryid.Value = DBNull.Value;
                }

                return await db.Database.SqlQuery<UserDetailVM>("exec [dbo].[GetActiveUserMappingCategoryWiseData]@country, @categoryid, @subcategoryid, @isactive, @isaway", new SqlParameter("@country", userCountry), sqlcategoryid, sqlsubcategoryid, new SqlParameter("@isactive", isactive), new SqlParameter("@isaway", isaway)).ToListAsync();

            }
        }
            
        public List<UserDetailVM> GetMailboxUsers(int mailboxId)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                return db.Database.SqlQuery<UserDetailVM>("exec[dbo]. [GetMailboxUsers] @MailboxId", new SqlParameter("@MailboxId", mailboxId)).ToList();
            }
        }
                    public UserDetailVM GetUserInfoByEmployeeID(string userCountry, string currentuserid)
        {
            var allUsers = GetAllUserDetails(userCountry);

            return allUsers.FirstOrDefault(u => u.EmployeeID == currentuserid);
        }

        public List<UserDetailVM> GetAllUserDetails(string userCountry)
        {
            List<UserDetailVM> users = new List<UserDetailVM>();
            if (HttpContext.Current != null && HttpContext.Current.Cache["AllUsers_" + userCountry] != null)
                return (List<UserDetailVM>)HttpContext.Current.Cache["AllUsers_" + userCountry];



            if (HttpContext.Current == null || HttpContext.Current.Cache["AllUsers_" + userCountry] == null)
            {



                SqlParameter country = new SqlParameter("@Country", userCountry);

                if (userCountry == null)
                {

                    country.Value = DBNull.Value;
                }

                using (CQCMSDbContext db = new CQCMSDbContext())
                    users = db.Database.SqlQuery<UserDetailVM>("exec [dbo]. [GetAllUserDetails] @country", country).ToList();
                SetUserDataToCache(userCountry, users);

            }
            return users;
        }



        private void SetUserDataToCache(string userCountry, List<UserDetailVM> users, string CacheKeyPrefix = "AllUsers_")
        {

            if (HttpContext.Current != null)

                HttpContext.Current.Cache.Add(CacheKeyPrefix + userCountry, users, null, DateTime.Now.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);

        }
    }
}
