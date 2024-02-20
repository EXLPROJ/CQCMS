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
using System.Web.Routing;


namespace CQCMS.API.Controllers
{
    public class AssignmentAPIController : ApiController
    {
        protected static Logger logger = LogManager.GetLogger("EmailEchoTransformation");
        [HttpGet]
        [System.Web.Http.AllowAnonymous]

        [System.Web.Http.Route("api/SetSLAandTouch/{CaseId}/{SubcategoryId}/{userCountry}/{CurrentUserId}/{AssignmentNeeded}")]

        public static void SetSLAandTouch(int CaseId, int Subcategoryld, string userCountry, string CurrentUserId, bool AssignmentNeeded = true)
        {

            //Set case 's subcategory & category
            //Update SLA &touch due date            
            //If subcategory has #tags
            //Invoke InheritHashTags(S / 3, tags from subcategory)            
            // If SkipAssignment = false,
            // Invoke AssignCase(pass IsSubCategoryChanged to true)



            var existingCase = new CaseAllData().GetCaseByCaseID(userCountry, CaseId);

            try
            {
                //to find sla due date
                string slahours = new CategoryData().GetSubCategorybySubCategoryID(userCountry, existingCase.SubCategoryID).SLA;

                if (slahours != null && slahours != "")
                {
                    string netSlaHours = (Convert.ToInt32(slahours) * existingCase.NoOfQueries).ToString();
                    existingCase.SLADueDate = HelperFunctions.SetSLAByCreatedOnDate(existingCase.Createdon, netSlaHours, userCountry);
                    var result = Task.Run(() => new CaseAllData().UpdateSLADueDatebyID(userCountry, DateTime.Now, CurrentUserId, existingCase.CaseID, existingCase.SLADueDate.Value)).Result;
                }

                existingCase.TouchDueDate = Task.Run(() => new CaseAllData().CalcTouchDueDate(CaseId)).Result;
                if (existingCase.TouchDueDate != null)
                {

                    var updated = Task.Run(() => new CaseAllData().UpdateTouchDueDatebyID(userCountry, DateTime.Now, CurrentUserId, existingCase.CaseID, existingCase.TouchDueDate.Value)).Result;
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        [HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/VerifyCaseAssignmentNeed")]
        public static bool VerifyCaseAssignmentNeed(CaseChangeDTO caseChangeDTO, string CurrentUserId)
        {
            try
            {

                var count = 0;

                List<string> MLCountries = new List<string>();
                if (ConfigData.GetConfigValue("MLCountries") != null && ConfigData.GetConfigValue("MLCountries") != "" && ConfigData.GetConfigValue("EnableMachineLearning").ToString().ToLower() == "true")
                {

                    MLCountries = ConfigData.GetConfigValue("MLCountries").ToString().Split(';').ToList();
                }

                CaseDetailVM caseDetail = new CaseAllData().GetCaseByCaseID(caseChangeDTO.Country, caseChangeDTO.CaseID);

                if (caseDetail.ParentCaseID != null && caseDetail.CaseIdIdentifier.Contains('#') == true)
                {

                    return false;
                }

                if (caseDetail.CurrentlyAssignedTo == null && caseDetail.PreviouslyAssignedTo == null)
                {

                    string savedhashtags = "";//new HashTagData().GetFamilyAndHashtagsNamellithCaseTd(caseChangeDTO.CaseID);

                    if (ConfigData.GetConfigValue("EnableMachineLearning").ToString().ToLower() == "true" && (caseDetail.Country == "global" || caseDetail.Country.Contains(";") || (MLCountries.Count > 0 && MLCountries.Contains(caseDetail.Country) && (caseDetail.SubCategoryID == 0 || caseDetail.SubCategoryID == null))) && (!savedhashtags.Contains("ExcludeFromAutoClassification")))
                    {


                        return false;
                    }



                    caseChangeDTO.IsNewCase = true;
                    new CaseAllData().InsertIntoCaseAuditTrailTable("Its a new case,Assigning the Case", caseDetail.CaseID, "System", "Routing");

                    return true;
                }




                string assigneduser = (caseDetail.CurrentlyAssignedTo == null ? caseDetail.PreviouslyAssignedTo : caseDetail.CurrentlyAssignedTo);

                if (assigneduser != null && assigneduser != "")
                {
                    UserDetailVM Userdetails = new UserData().GetUserInfoByEmployeeIDCountry(caseChangeDTO.Country, assigneduser);

                    if (Userdetails != null)

                    {

                        if (Userdetails.IsOOO == true)
                        {

                            caseChangeDTO.IsUserOOO = true;
                            new CaseAllData().InsertIntoCaseAuditTrailTable("Current user is OOO, Assigning the case", caseDetail.CaseID, "System", "Routing");

                            return true;

                        }

                        if (Userdetails.IsAway == true)
                        {
                            bool hasrollover = new CaseAllData().CheckIfCaseHasRollover(caseDetail.CaseID);
                            if (hasrollover == true && caseDetail.NewEmailcount > 0)

                            {

                                caseChangeDTO.IsNewEmailOnRolloverCase = true;

                                new CaseAllData().InsertIntoCaseAuditTrailTable("User is Away & Case has Rollover tag, Assigning the case", caseDetail.CaseID, "System", "Routing");

                                return true;

                            }
                        }
                    }

                    else
                    {
                        return true;
                    }
                }
                string user = caseDetail.CurrentlyAssignedTo;

                if (user != null)
                {
                    UserDetailVM Userdetails = new UserData().GetUserInfoByEmployeeID(caseChangeDTO.Country, user);
                    EmailVM email = Task.Run(() => new EmailData().GetEmailbyEmailIdAsync(caseDetail.Country, caseDetail.LastEmailID)).Result;
                    if (Userdetails.IsReviewRequired == true && email.EmailSubject.Contains("DRAFT"))
                    {

                        caseChangeDTO.IsReviewNeeded = true;
                        return true;

                    }
                }
                foreach (var property in caseChangeDTO.GetType().GetProperties())
                {
                    var propertyname = "";


                    if (property.Name == "IsNlewCase" || property.Name == "IsCaseClassifiedforFirstTime" || property.Name == "Reclassified" || property.Name == "IsHashTagChanged")
                    {
                        if ((bool)property.GetValue(caseChangeDTO, null) == true)
                        {
                            switch (property.Name)
                            {
                                case "IsNewCase": propertyname = "Its a new case, assigning the case to an user"; break;
                                case "Reclassified": propertyname = "Case has been reclassified,assigning the case to an user"; break;
                                case "IsCaseClassifiedforFirstTime": propertyname = "Case is classified for the first time ,assigning the case to an user"; break;
                                case "IsHashTagChanged": propertyname = "HashTag has been changed, assigning the case to an user"; break;
                            }
                            new CaseAllData().InsertIntoCaseAuditTrailTable(propertyname, caseDetail.CaseID, "System", "Routing");
                            count++;
                            break;
                        }
                    }
                }

                if (count > 0)
                {
                    return true;
                }

                else { return false; }
            }


            catch (Exception ex)
            {

                //logger.Info("error in verify assignmentneeded ");
                return false;
            }
        }



        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/AssignCase")]
        public dynamic AssignCase(CaseChangeDTO caseChangeDTO)
        {

            string recommendedUserId;
            UserDetailVM recommendedUser = null;
            bool enforceReassignment = false;
            UserDetailVM persontoassign = null;
            List<string> countrylist = null;
            bool IsCurrrentUserIsInactive = false;

            var currentUser = new UserData().GetUserInfoByEmployeeIDCountry(caseChangeDTO.Country, caseChangeDTO.CurrentUserId);

            if (currentUser != null && currentUser.IsActive == false && currentUser.IsOOO == true)
                IsCurrrentUserIsInactive = true;
            CaseDetailVM assignCase = new CaseAllData().GetCaseByCaseID(caseChangeDTO.Country, caseChangeDTO.CaseID);
            bool isRolloverEligible = new CaseAllData().CheckIfCaseHasRollover(caseChangeDTO.CaseID);
            new UsersAPIController().Onetimeupdatecapacity();// calculating capacity of all user before assignment
            logger.Info(" Entered to assignCase()/ CaseId = " + assignCase.CaseID + "; CurrentlyAssignedTo = " + assignCase.CurrentlyAssignedTo + ";CurrentUserId = " + (caseChangeDTO.CurrentUserId == null ? "From BOT" : caseChangeDTO.CurrentUserId));

            recommendedUserId = assignCase.CurrentlyAssignedTo;

            if ((assignCase.KeepWithMe == false || assignCase.KeepWithMe == null) && (assignCase.KeepWithMe == true && caseChangeDTO.CurrentUserId == Environment.UserName || IsCurrrentUserIsInactive == true))
            {
                if (recommendedUserId != null)
                {
                    recommendedUser = new UserData().GetUserInfoByEmployeeIDCountry(caseChangeDTO.Country, recommendedUserId);
                    if (recommendedUser != null)
                    {
                        logger.Info(" is currentlyassigned isooo/ =" + recommendedUser.IsOOO + "; isAway" + recommendedUser.IsAway + ";IsRoll0verEligible = " + isRolloverEligible);
                    }
                }
                if ((recommendedUser == null) || (recommendedUser.IsOOO == true) || recommendedUser.IsActive == false || (recommendedUser.IsAway == true && isRolloverEligible == true))
                {
                    recommendedUser = null;
                }
                if (recommendedUser == null && !String.IsNullOrEmpty(assignCase.PreviouslyAssignedTo))
                {
                    recommendedUserId = assignCase.PreviouslyAssignedTo;
                    recommendedUser = new UserData().GetUserInfoByEmployeeIDCountry(caseChangeDTO.Country, recommendedUserId);

                    if ((recommendedUser == null) || (recommendedUser.IsOOO == true) || recommendedUser.IsActive == false || (recommendedUser.IsAway == true && isRolloverEligible == true))
                    {
                        recommendedUser = null;
                    }
                }
                if (recommendedUser == null)
                {
                    enforceReassignment = true;
                }
                //Added for review required cases to be reassigned
                else if (recommendedUser.IsReviewRequired != null && recommendedUser.IsReviewRequired == true)
                {
                    logger.Info("IsReviewRequired-true");
                    enforceReassignment = true;
                    caseChangeDTO.EnforceMakerChecker = true;
                }

                List<UserDetailVM> EligibleUsers = FindAllEligiblePersons(caseChangeDTO.Country, caseChangeDTO.CaseID, enforceReassignment, caseChangeDTO.EnforceMakerChecker, recommendedUserId);

                if (EligibleUsers.Count() != 0)

                {
                    foreach (UserDetailVM u in EligibleUsers)
                    {
                        logger.Info("FindEligiblepersons =:" + u.EmployeeID + " CaseId: " + assignCase.CaseID + " Weigntedcapacity-" + u.WeightedCapacity);
                    }
                }
                if (EligibleUsers != null && EligibleUsers.Any(u => u.EmployeeID == assignCase.CurrentlyAssignedTo))
                {
                    new CaseAllData().InsertIntoCaseAuditTrailTable("Currently Assigned user is the most eligible user among all .Keeping the case with the same person", caseChangeDTO.CaseID, "System", "Routing");
                    return assignCase;
                }
                if (EligibleUsers != null && EligibleUsers.Any(u => u.EmployeeID == assignCase.PreviouslyAssignedTo))
                {
                    if (EligibleUsers.Count() > 1)
                        new CaseAllData().InsertIntoCaseAuditTrailTable("previously Assigned user is the most eligible user among all .", caseChangeDTO.CaseID, "System", "Routing");
                    persontoassign = EligibleUsers.Where(u => u.EmployeeID == assignCase.PreviouslyAssignedTo).FirstOrDefault();
                }
                if (persontoassign == null)
                {
                    if (assignCase.Country.Contains(";"))
                    {
                        countrylist = Convert.ToString(assignCase.Country).Split(',').ToList();
                        persontoassign = EligibleUsers.Where(y => countrylist.Any(b => y.Country.Contains(b))).OrderBy(x => x.WeightedCapacity).FirstOrDefault();
                    }
                    else
                        persontoassign = EligibleUsers.OrderBy(x => x.WeightedCapacity).FirstOrDefault();
                    if (persontoassign != null && EligibleUsers.Count() > 1)
                    {
                        new CaseAllData().InsertIntoCaseAuditTrailTable("Assigning the case based on weighted capacity", caseChangeDTO.CaseID, "System", "Routin");
                    }
                    //here require to change in case of global
                }

                if (persontoassign == null && (caseChangeDTO.CurrentUserId != null && !caseChangeDTO.CurrentUserId.Contains("THOR") && !caseChangeDTO.CurrentUserId.Contains("BOLT")) && ConfigData.GetConfigValue("AssignCaseToCategorizer", assignCase.Country) == "True")
                {
                    if (IsCurrrentUserIsInactive == false && currentUser != null)
                    {
                        new CaseAllData().InsertIntoCaseAuditTrailTable("Assigning the case to the current user", caseChangeDTO.CaseID, "System", "Routing");
                        persontoassign = currentUser;
                    }
                }
            }
            else
            {
                if (IsCurrrentUserIsInactive == false && currentUser != null)
                {
                    new CaseAllData().InsertIntoCaseAuditTrailTable("Keep with me flag is set to true, Assigning the case to the current user", caseChangeDTO.CaseID, "System", "Routing");
                    persontoassign = currentUser;
                }
            }

            //Final block of assigning if person found else move it to unassigned
            if (persontoassign != null)
            {

                CaseAssignedTime UpdateCaseAssigned = new CaseAssignedTime();
                UpdateCaseAssigned.CaseID = assignCase.CaseID;
                UpdateCaseAssigned.LastActedBy = caseChangeDTO.CurrentUserId;
                UpdateCaseAssigned.AssignedTime = DateTime.Now;
                UpdateCaseAssigned.LastActedOn = DateTime.Now;
                UpdateCaseAssigned.IsAssigned = true;

                if (!assignCase.IsCaseClosed)

                {

                    if (assignCase.CurrentlyAssignedTo != null)
                        UpdateCaseAssigned.PreviouslyAssignedTo = assignCase.CurrentlyAssignedTo;
                    UpdateCaseAssigned.CurrentlyAssignedTo = persontoassign.EmployeeID;
                }
                else if (assignCase.IsCaseClosed)
                {
                    UpdateCaseAssigned.PreviouslyAssignedTo = persontoassign.EmployeeID;

                    UpdateCaseAssigned.CurrentlyAssignedTo = null;

                }
                new CaseAllData().InsertIntoCaseAuditTrailTable("Case is assigned to " + persontoassign.EmployeeName, caseChangeDTO.CaseID, "System", "Routing");
                assignCase = Task.Run(() => new CaseAllData().UpdateCaseAssignedTime(caseChangeDTO.Country, UpdateCaseAssigned)).Result;
                assignCase = Task.Run(() => new CaseAllData().UpdateCaseAssignAttempts(caseChangeDTO.Country, assignCase.CaseID, DateTime.Now,
                Environment.UserName, Convert.ToInt32(assignCase.CaseAssignAttempts) + 1)).Result;
                assignCase.EmployeeName = persontoassign.EmployeeName;
                assignCase.CurrentlyAssignedTo = UpdateCaseAssigned.CurrentlyAssignedTo;

            }
            else if (persontoassign == null && (assignCase.CurrentlyAssignedTo != null || (assignCase.IsCaseClosed == true && assignCase.PreviouslyAssignedTo != null)))
            { //When user is 000,no user found as backup,| cases should go to unassigned)

                logger.Info("Releasing case to unassigned DB ;User Isooo/Away&&Rolovereligible");

                ReleaseCaseDTO relasecases = new ReleaseCaseDTO

                {
                    CaseId = caseChangeDTO.CaseID,
                    ReleaseToUser = null,
                    ReleaseFromUser = assignCase.CurrentlyAssignedTo,
                    Country = caseChangeDTO.Country,
                    CurrentUserId = caseChangeDTO.CurrentUserId,
                };

                string result = Task.Run(() => new CaseAPIController().ReleaseCases(relasecases)).Result;

                assignCase.CurrentlyAssignedTo = null;

                assignCase.EmployeeName = null;

                new CaseAllData().InsertIntoCaseAuditTrailTable("No eligibe user found,Case move to unassigned ", caseChangeDTO.CaseID, "System", "Routing");

            }

            new UsersAPIController().Onetimeupdatecapacity();// calculating capacity of all user before assignment

            return assignCase;

        }




        [HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/FindAllE1igiblePersons/{Country}/{Caseld}/{enforceReassignment}/{enforceMakerChecker}/{CurrentUserId}")]
        public static List<UserDetailVM> FindAllEligiblePersons(string Country, int CaseId, bool enforceReassignment,
        bool enforceMakerChecker, string CurrentUserId)
        {

            List<UserDetailVM> findAllPersons = null;

            UserDetailVM personToAssign = null;

            bool hasmailboxchecked = false;

            CaseDetailVM assignCase = new CaseAllData().GetCaseByCaseID(Country, CaseId);
            bool IsNewCase = assignCase != null && assignCase.FirstEmailID == assignCase.LastEmailID;

            // || assignCase.CaseStatusID

            bool IsMLRecommended = assignCase != null && assignCase.NOQ_1 != null && assignCase.SubCategoryID != null && (assignCase.CaseStatusID == 1 || assignCase.CaseStatusID == 3);

            bool isRolloverEligible = new CaseAllData().CheckIfCaseHasRollover(CaseId);
            //Check First if user isooo/Away
            if ((assignCase.Country != "global" && !assignCase.Country.Contains(";")) && (findAllPersons == null) && (assignCase.CurrentlyAssignedTo != null || (assignCase.PreviouslyAssignedTo != null && assignCase.IsCaseClosed == true)))

            {
                UserDetailVM user = new UserData().GetUserInfoByEmployeeIDCountry(assignCase.Country, assignCase.CurrentlyAssignedTo);
                if (user == null && assignCase.IsCaseClosed == true)
                    user = new UserData().GetUserInfoByEmployeeIDCountry(assignCase.Country, assignCase.PreviouslyAssignedTo);
                if (user != null)
                {
                    if ((user.IsOOO == true) || (user.IsActive == false) || (user.IsAway == true && IsNewCase) || (user.IsAway == true && isRolloverEligible))
                    {
                        findAllPersons = new List<UserDetailVM>();
                        findAllPersons.Add(user);
                        findAllPersons = ReplaceUnavailableUser(findAllPersons, (IsNewCase || isRolloverEligible), enforceMakerChecker, CurrentUserId);
                        if (findAllPersons.Count() == 0)
                            new CaseAllData().InsertIntoCaseAuditTrailTable("No backup user found", CaseId, "System", "Routing");
                        else
                            new CaseAllData().InsertIntoCaseAuditTrailTable("Found " + findAllPersons.Count() + " backup user", CaseId, "System", "Routing");
                    }
                }
            }
            if ((findAllPersons == null || findAllPersons.Count() == 0) && (assignCase.SubCategoryID == null || assignCase.SubCategoryID == 0))
            {
                findAllPersons = new UserData().GetMailboxUsers(assignCase.MailboxID);
                findAllPersons = ReplaceUnavailableUser(findAllPersons, (IsNewCase || enforceReassignment), enforceMakerChecker, CurrentUserId);
                if (findAllPersons.Count() == 0)
                    new CaseAllData().InsertIntoCaseAuditTrailTable("No user found from mailbox access", CaseId, "System", "Routing");
                else
                    new CaseAllData().InsertIntoCaseAuditTrailTable("Found " + findAllPersons.Count() + "user from mailbox access", CaseId, "System", "Routing");
                hasmailboxchecked = true;
            }
            //todo: if case has user inputed hashtag that should skip casetype based routing
            if ((findAllPersons == null || findAllPersons.Count() == 0) && assignCase.SubCategoryID != 0 && assignCase.SubCategoryID != null)

            {
                findAllPersons = Task.Run(() => new UserData().GetActiveUserMappingCategoryWiseDataAsync(Country, assignCase.CategoryID, assignCase.SubCategoryID, true, !(IsNewCase || enforceReassignment))).Result;
                findAllPersons = ReplaceUnavailableUser(findAllPersons, (IsNewCase || enforceReassignment), enforceMakerChecker, CurrentUserId);
                if (findAllPersons.Count() == 0)
                    new CaseAllData().InsertIntoCaseAuditTrailTable("No user found from case type access", CaseId, "System", "Routing");
                else
                    new CaseAllData().InsertIntoCaseAuditTrailTable("Found " + findAllPersons.Count() + "user from Case type access", CaseId, "System", "Routing");
            }
            if ((findAllPersons == null || findAllPersons.Count() == 0) && (assignCase.Country != "global" && !assignCase.Country.Contains(";")))
            {
                try
                {
                    int userId = new UserData().FindEligibleUserBasedOnHashTagIds(CaseId);
                    if (userId != 0)
                    {

                        personToAssign = new UserData().GetUserbyIDFirstOrDefault(Country, userId);
                    }

                    if (personToAssign != null)
                        findAllPersons.Add(personToAssign);

                    if (findAllPersons.Count() == 0)
                        new CaseAllData().InsertIntoCaseAuditTrailTable("No user found from hashTag rule evaluation", CaseId, "System", "Routing");


                }

                catch (Exception ex)
                {

                    logger.Info("Error in getting user from hash tag :" + ex.Message);

                }
            }

            if (findAllPersons == null || findAllPersons.Count() == 0)
            {


                // findAllPersons =CoRoData.GetCoroCSSAMUsers(Country, assignCase);
                findAllPersons = ReplaceUnavailableUser(findAllPersons, (IsNewCase || enforceReassignment), enforceMakerChecker, CurrentUserId);

                if (findAllPersons.Count() == 0)
                    new CaseAllData().InsertIntoCaseAuditTrailTable("No user found from CORO CSS/AM", CaseId, "System", "Routing");
                else
                    new CaseAllData().InsertIntoCaseAuditTrailTable("Found " + findAllPersons.Count() + " user from CSS/AM ", CaseId, "System", "Routing");

            }

            //ML Implementation change
            if ((findAllPersons == null || findAllPersons.Count() == 0) && (IsNewCase || IsMLRecommended) && assignCase.CurrentlyAssignedTo == null && hasmailboxchecked == false)


            {

                findAllPersons = new UserData().GetMailboxUsers(assignCase.MailboxID);
                findAllPersons = ReplaceUnavailableUser(findAllPersons, (IsNewCase || enforceReassignment), enforceMakerChecker, CurrentUserId);
                if (findAllPersons.Count() == 0)
                    new CaseAllData().InsertIntoCaseAuditTrailTable("No user found from mailbox access", CaseId, "System", "Routing");
                else
                    new CaseAllData().InsertIntoCaseAuditTrailTable("Found " + findAllPersons.Count() + " user from mailbox access", CaseId, "System", "Routing");

               
            }
            return findAllPersons;

        }






        [HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api /ReplaceUnavai lableUser/{Users}/{ReplaceAwayUsers}/{enforceMakerChecker}/{CurrentUserId}")]
        public static List<UserDetailVM> ReplaceUnavailableUser(List<UserDetailVM> Users, bool ReplaceAwayUsers, bool enforceMakerChecker, string CurrentUserId)

        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                if (Users.Count != 0)
{
                    List<UserDetailVM> bkpusers = new List<UserDetailVM>();
                    List<UserDetailVM> ooouserstoremove = new List<UserDetailVM>();
                    foreach (var f in Users)
                    {
                        if (f.IsOOO == true || (f.IsAway == true && ReplaceAwayUsers == true) || f.IsActive == false)
                        {
                            //oackup user
                            var temp = db.BackupUserDetails.Where(x => x.UserID == f.UserID).ToList();
                            if (temp.Count != 0)
                            {
                                foreach (var c in temp)
                                {
                                    UserDetailVM addbkp = new UserData().GetUserDetailsByID(c.BackupUserID, f.Country);
                                    if (addbkp != null)
                                    {
                                        bkpusers.Add(addbkp);
                                    }
                                }
                            }
                            ooouserstoremove.Add(f);
                        }
                    }
                    //todo :finding backup user based on case country
                    //remove the 000 user from list, cannot remove inside the loop
                    if (ooouserstoremove.Count != 0)
                    {
                        foreach (var ouser in ooouserstoremove)
                        {
                            Users.Remove(ouser);
                        }
                    }
                    if (bkpusers.Count != 0)
                    {
                        foreach (var b in bkpusers)
                        {
                            //only add if the user is not already on the list
                            if (!Users.Any(x => x.EmployeeEmailID.ToLower().Equals(b.EmployeeEmailID.ToLower())))
                            {
                                if (b.IsOOO == false && b.IsAway == false && b.IsActive == true) //only add if the backup user is not 000 and not Away
                                {
                                    Users.Add(b);
                                }
                            }
                        }
                    }
                    if (enforceMakerChecker && !string.IsNullOrEmpty(CurrentUserId))
                    {
                        List<UserDetailVM> reviewpersontoremove = new List<UserDetailVM>();
                        Users.RemoveAll(u => u.EmployeeID == CurrentUserId);
                        foreach (var x in Users)
                        {
                            if (x.IsReviewRequired)
                            {
                                reviewpersontoremove.Add(x);
                            } }
                        if (reviewpersontoremove.Count != 0)
                        {
                            foreach (var ouser in reviewpersontoremove)
                            {
                                Users.Remove(ouser);
                            }
                        }
                    }
                }
                return Users;
            }
        }

    }
}
                    