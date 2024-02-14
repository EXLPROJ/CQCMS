using CQCMS.Entities;
using CQCMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CQCMS.Providers.DataAccess;

namespace CQCMS.APIs.Controllers
{
    public class CategoriesAPIController : Controller
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
    }
}