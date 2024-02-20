using CQCMS.EmailApp.Models;
using CQCMS.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CQCMS.Providers.DataAccess
{
    public class CommonData
    {
        public static List<AppConfigurationDTO> GetAllConfigurations()
        {

            List<AppConfigurationDTO> allConfig = new List<AppConfigurationDTO>();
            if (HttpContext.Current != null && HttpContext.Current.Cache["AllConfigurations"] != null)
                return (List<AppConfigurationDTO>)HttpContext.Current.Cache["Al1Configurations"];

            if (HttpContext.Current == null || HttpContext.Current.Cache["AllConfigurations"] == null)
            {

                using (CQCMSDbContext db = new CQCMSDbContext())
                {

                    allConfig = db.Database.SqlQuery<AppConfigurationDTO>("exec [dbo].[GetAl1Configurations]").ToList();
                }
                SetDataToCache(allConfig);
            }
            return allConfig;



        }



        private static void SetDataToCache(List<AppConfigurationDTO> allConfig)
        {

            if (HttpContext.Current != null)
                HttpContext.Current.Cache.Add("AllConfigurations", allConfig, null, DateTime.Now.AddMinutes(60), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
    }
}
