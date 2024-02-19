using CQCMS.Entities;
using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CQCMS.Providers.DataAccess;
using System.Web.Http;

namespace CQCMS.APIs.Controllers
{
    public class CategoriesAPIController : ApiController
    {
        // GET: CategoriesAPI

        [HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/GetCategoryDataForCountry/{UserCountry}")]
        public dynamic GetCategoryDataForCountry(string userCountry)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                List<CategoryVM> categoriesDropDown = new List<CategoryVM>();
                categoriesDropDown = Task.Run(() => new CategoryData().GetAllCategoryAsync(userCountry)).Result;
                return categoriesDropDown.OrderBy(x => x.CategoryName).ToList();
            }
        }

        [HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/GetSubCategoryData/{CategoryId}/{CaseId}/{Country}")]
        public dynamic GetSubCategoryData(int? CategoryId, int? CaseId, string Country)
        {
            if (CategoryId != null)
            {
                //CaseDetailvM caseDetail = new CaseAllData().GetCaseByCaseIDForVirtual(usercountry, caseid);
                //int? mailboxid = null;// Task.Run(() => new CaseAllData().GetMailboxIDByCaseID(Country, CaseId)).Result;
                var multidata = Task.Run(() => new CategoryData().GetSubCategoryByCategoryID(Country, CategoryId)).Result;
                //if (mailboxid != null && mailboxid != 0)
                //{
                //    List<SubCategoryDisplayVM> applicableList = new List<SubCategoryDisplayVM>();
                //    var mailboxClientProcessRows = Task.Run(() => new MailboxClientmappingData().GetMailboxclientProcessMapByMailboxID(mailboxid)).Result;
                //    if (mailboxClientProcessRows != null && mailboxClientProcessRows.Count > @)

                //    mailboxClientProcessRows = mailboxClientProcessRows.Where(x => x.CategoryID == CategoryId).ToList();
                //    if (mailboxClientProcessRows != null && mailboxClientProcessRows.Count > 0)

                //        foreach (var row in mailboxClientProcessRows)
                //        {

                //            applicableList.AddRange(multidata.where(x => x.SubCategoryID == row.SubCategoryID).ToList());
                //}
                //}
                //}
                //            if (applicableList.count > 0)
                //                multidata = applicableList;
                return multidata.OrderBy(x => x.SubCategoryName).ToList();
            }
            return null;
        }

        [HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/GetLookupValuesByLookupNameCQCMS/{ManagedLookupName}/{UserCountry}")]
        public dynamic GetLookupValuesByLookupNameCQCMS(string ManagedLookupName, string UserCountry)
        {
            List<LookupValuesCQCMS> lookupValues = new List<LookupValuesCQCMS>();
            lookupValues = new CommonData().GetLookupValuesByLookupNameCQCMS(ManagedLookupName, UserCountry);
            var valueList = lookupValues.Select(p => new { Text = p.LookupValue, Value = p.LookupValueID.ToString() }).ToList();
            return valueList;
        }

        
    }
}