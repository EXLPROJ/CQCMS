﻿using CQCMS.Entities.Models;
using CQCMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CQCMS.Providers.DataAccess;

namespace CQCMS.APIs.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/GetName/{Name}")]
        public string GetName(string Name)
        {
            return "Hello Amit from API" + Name;
        }

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
