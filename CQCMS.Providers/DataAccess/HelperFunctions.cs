using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;
using HtmlAgilityPack;
using CQCMS.Entities.Models;


namespace CQCMS.Providers.DataAccess
{
    public class HelperFunctions

    {
        Logger logger = LogManager.GetLogger("EmailEchoTransformation");

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




        public static DateTime SetSLAByCreatedOnDate(DateTime CreatedOnDate, string netSlaHours, string country = null)
        {
            DateTime SLADueDate;
            DateTime dtl = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 15, 0, 0); //TODAY 3PM
            DateTime dt2 = new DateTime(DateTime.Today.AddBusinessDays("1", country).Year, DateTime.Today.AddBusinessDays("1", country).Month, DateTime.Today.AddBusinessDays("1", country).Day, 8, 0, 0); //NEXT DAY 8AM

            int res = DateTime.Compare(CreatedOnDate, dtl);

            if (res < 0) // comes before 3pm

            {
                SLADueDate = CreatedOnDate.AddBusinessDays(netSlaHours, country);
            }
            else // comes after 3pm
            {
                SLADueDate = dt2.AddBusinessDays(netSlaHours, country);
            }

            return SLADueDate;
        }
    }
}




public static class Extensions
{
    public static DateTime AddBusinessDays(this DateTime current, string hours, string Country = null)
    {
        try
        {
            double SLAhours = 0;

            if (hours != "")

            {
                SLAhours = Convert.ToDouble(hours);
            }

            int days = Convert.ToInt32(Math.Ceiling(SLAhours / 24.0));
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    current = current.AddDays(sign);
                }
                while (current.DayOfWeek == DayOfWeek.Saturday ||
                //current .DayOfWeek == DayOfWeek.Sunday || IsHoliday(current));
                current.DayOfWeek == DayOfWeek.Sunday
                //|| IsHoliday(current, Country)
                );
            }
        }
        catch (Exception ex)
        {
        }

        return current;
    }




    public static bool IsHoliday(this DateTime originalDate, string Country = null)
    {
        // string userCountry = (string)System.Web.HttpContext.Current.Session["UserCountry”];

        using (var CoRoProvider = new HolidayDBContext())
        {
            if (Country.ToUpper() == "GLOBAL" || Country.ToUpper().Contains(";"))
            {
                //if (CoRoProvider.Holidays.FirstOrDefault(x => x.Date == originalDate.Date == null)
                return false;
            }
            else
            {
                if (CoRoProvider.Holidays.FirstOrDefault(x => x.Date == originalDate.Date && x.Geography == Country) == null)
                    return false;
            }
            return true;
        }
    }
}
