using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CQCMS.Dashboard.Controllers
{
    public class CaseDataController : Controller
    {
        // GET: CaseData
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CaseDataLight()
        {
            return View("CaseDataLight");
        }
    }
}