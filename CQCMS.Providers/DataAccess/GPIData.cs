using CQCMS.EmailApp.Models;
using CQCMS.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CQCMS.Providers.DataAccess
{
    public class GPIData
    {
        public void UpdateGPILastEmailIdInCase(int CaseId) {
            try
            {

                using (CQCMSDbContext db = new CQCMSDbContext())
                {
                    string exeQuery = "exe [dbo].[UpadteGPILastEmailIDInCAse] @CaseID";
                    db.Database.ExecuteSqlCommand(exeQuery,new SqlParameter("@CaseID", CaseId));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

               


 
