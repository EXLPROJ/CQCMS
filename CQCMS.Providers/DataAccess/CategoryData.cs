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
    public class CategoryData
    {
        public async Task<CategoryVM> GetCategorybyCategoryID(string userCountry, int? categoryID)
        {

            {

                return GetCategorybyCategoryIDBabyCase(userCountry, categoryID);
            }
        }
        public static CategoryVM GetCategorybyCategoryIDBabyCase(string userCountry, int? categoryID)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Cache["AllCategories_" + userCountry] != null)
                {

                    return ((List<CategoryVM>)HttpContext.Current.Cache["AllCategories_" + userCountry]).FirstOrDefault(x => x.CategoryID == categoryID);
                }
            }

            SqlParameter sqlcategoryID = new SqlParameter("@categoryID", categoryID);
            if (categoryID == null)

            {
                sqlcategoryID.Value = DBNull.Value;

            }
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                return db.Database.SqlQuery<CategoryVM>("exec [dbo].[getCategorybyCategoryID] @country ,@categoryID", new SqlParameter("@country", userCountry), sqlcategoryID).FirstOrDefault();

            }
        }

        public SubCategoryVM GetSubCategorybySubCategoryID(string userCountry, int? subCategoryID)
        {



            return GetSubCategorybySubCategoryIDBabyCase(userCountry, subCategoryID);
        }

        public async Task<SubCategoryVM> GetSubCategorybySubCategoryIDAsync(string userCountry, int? subCategoryID)
        {

            return GetSubCategorybySubCategoryIDBabyCase(userCountry, subCategoryID);
        }



        public static SubCategoryVM GetSubCategorybySubCategoryIDBabyCase(string userCountry, int? subCategoryID)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Cache["Al1SubCategories_" + userCountry] != null)
                {
                    return ((List<SubCategoryVM>)HttpContext.Current.Cache["Al1SubCategories_" + userCountry]).FirstOrDefault(x => x.SubCategoryID == subCategoryID);
                }
            }
            SqlParameter sqlsubCategoryID = new SqlParameter("@subCategoryID", subCategoryID);
            if (subCategoryID == null)
            {
                sqlsubCategoryID.Value = DBNull.Value;
            }
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                return db.Database.SqlQuery<SubCategoryVM>("exec [dbo].[GetSubCategorybySubCategoryID] @country,@subCategoryID", new SqlParameter("@country", userCountry), sqlsubCategoryID).FirstOrDefault();
            }
           
        }
             
                    
    }
}
