using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CQCMS.Providers.DataAccess
{
    public class ConfigData
    {

        public static Dictionary<string, string> DBAppSettings = new Dictionary<string, string>();

        public static Dictionary<string, string> InitializeAppSettings()

        {
            var list = CommonData.GetAllConfigurations();
            foreach (var item in list)
            {
                if (DBAppSettings.ContainsKey(item.Key))
                {
                    DBAppSettings[item.Key] = item.Value;
                }
                else
                {
                    DBAppSettings.Add(item.Key, item.Value);

                }
            }
            return DBAppSettings;
        }

        public static string GetConfigValue(string key, string Country="")
                {
            if ((HttpContext.Current == null || HttpContext.Current.Cache["AllConfigurations"] == null))
                InitializeAppSettings();
            if (Country != "" && Country != null && !Country.Contains(';'))
            {
                var keycountry = key + '_' + Country;
                if (DBAppSettings.ContainsKey(keycountry))
                    return DBAppSettings[keycountry];

            }
            return DBAppSettings.ContainsKey(key) == false ?
            null : DBAppSettings[key];
        }
    }
}
