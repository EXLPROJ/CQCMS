using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.Providers
{
    public class HelperFunctions
    {
        public static List<object> CreateParameterListfromModelWithoutIdout(object model)
        {

            var param = new List<object>();
            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                if (property.GetValue(model, null) == null || (property.GetValue(model, null).ToString() == "0" && property.Name == "CaseID"))
                {
                    param.Add(new SqlParameter("@" + property.Name, DBNull.Value));
                }
                else
                {
                    param.Add(new SqlParameter("@" + property.Name, property.GetValue(model, null)));
                }

            }
            return param;
        }
    }
}
