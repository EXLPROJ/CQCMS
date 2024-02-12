using CQCMS.EmailApp.Models;
using CQCMS.Entities.DTOs;
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
    public class CommonData
    {
        public static List<AppConfigurationDTO> GetaAllconfigurations()
        {

            List<AppConfigurationDTO> allConfig = new List<AppConfigurationDTO>();
            if (HttpContext.Current != null && HttpContext.Current.Cache["AllConfigurations"] != null)
                return (List<AppConfigurationDTO>)HttpContext.Current.Cache["AllConfigurations"];
            if (HttpContext.Current == null || HttpContext.Current.Cache["AllConfigurations"] == null)
            {
                using (CQCMSDbContext db = new CQCMSDbContext())
                {
                    allConfig =  db.Database.SqlQuery<AppConfigurationDTO>("exec [dbo].[GetAllConfigurations]").ToList();

                }
                SetDataToCache(allConfig);
            }
            return allConfig;
        }
        private static void SetDataToCache(List<AppConfigurationDTO> allConfig)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Cache.Add("AllConfigurations", allConfig, null, DateTime.Now.AddMinutes(60),
                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);

        }
        public List<LookupValues> GetLookupValuesByLookupName(string ManagedLookupName, string Country)
        {
            List<LookupValues> lookupValues = new List<LookupValues>();
            if (ManagedLookupName != null)
            {
                try
                {
                    using (CQCMSDbContext db = new CQCMSDbContext())
                    {
                        lookupValues = db.Database.SqlQuery<LookupValues>("exec [dbo].[GetAllLookupValuesByLookupName] @ManagedLookupName, @Country",new SqlParameter("@ManagedLookupName", ManagedLookupName),new SqlParameter("@Country", Country)).ToList();
                    }
                }
                catch (Exception ex)
                { }
            }
            return lookupValues;
        }
    }
}
