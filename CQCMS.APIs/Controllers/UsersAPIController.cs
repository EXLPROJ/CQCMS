//using CQCMS.Providers;
using CQCMS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Http = System.Web.Http;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Configuration;
//using .EmailProvider;
using NLog;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Http.Results;
using HtmlAgilityPack;
using CQCMS.CommonHelpers;
//using CQCMS.CPR;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using CQCMS.Entities.DTOs;
using System.Web.Mvc;
using System.Reflection;
using System.Web;
using System.Net;
using System.Web.Http.ExceptionHandling;
using System.Web.Helpers;
using CQCMS.Entities.Models;
using CQCMS.Providers.DataAccess;
using System.Security.Policy;
using Microsoft.Ajax.Utilities;
//using System.EnterpriseServices;
using System.Net.Mail;
//using System.Web.Services.Description;
using static System.Data.Entity.Infrastructure.Design.Executor;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
//using System.Web.Http.Common;

namespace CQCMS.API.Controllers
{
    public class UsersAPIController : ApiController
    {
        // GET: UsersAPI

        protected static Logger logger = LogManager.GetLogger("EmailEchoTransformation");

        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/CapacityUpdateforAllUsers")]
        public void Onetimeupdatecapacity()
        {
            //BeatLogger .RegisterEventStart("BOLTEmailInjectionCapacity”);
            logger.Info("Before setting all users capacity");
            new UserData().CapacityUpdateForAllUsers();
            logger.Info("After setting all users capacity");
            //BeatLogger .RegisterEventEnd("BOLTEmailInjectionCapacity”) ;
        }
    }
}