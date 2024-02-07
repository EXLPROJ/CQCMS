//using CQCMS.Providers;
using CQCMS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Http = System.Web.Http;
using System.Ling;
using System.Data;
using System.Configuration;
using System.Data.Entity;
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

namespace CQCMS.API.Controllers
{
    public class EmailAPIController : Controller
    {
        private static Logger logger = LoggerManager.GetLogger("EmailTransformation");
        // GET: EmailAPI
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/SaveNewEmailAndCreateCase")]
        public dynamic SaveNewEmailAndCreateCase(CaseAndEmailUpdateDTO updateDTO)
        {
            CaseIEmailIdDTO CaseIEmailIdDTO;
            if (updateDTO.UpdateEmail.Direction == "Incoming")
                CaseIEmailIdDTO = SaveNewInboundEmailAndCreateCase(updateDTO);
            else
                CaseIEmailIdDTO = SaveNewOutboundEmailAndCreateCase(updateDTO);
            return CaseIEmailldDTO;

        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/SaveNewOutboundEmailAndCreateCase")]

        public dynamic SaveNewOutboundEmailAndCreateCase(CaseAndEmailupdateDTO updateDTO)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                CaseIEmailIdDTO CaseIEmailIdDTO;
                Email email = null;

                var currentUser = new UserData().GetUserInfoByEmployeeID(updateDTO.UpdateEmail.Country, updateDTO.CurrentUserId);
                var mailboxaddress = new MailboxData().GetMailboxbyID(updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.MailboxID).MailboxName;
                if (currentUser != null && currentUser.EmployeeName != "" && mailboxaddress != null && mailboxaddress != "")

                {
                    updateDTO.UpdateEmail.EmailFrom = currentUser.EmployeeName + " <" + mailboxaddress + ">";
                }
                try
                {

                    email = SaveEmail(updateDTO.UpdateEmail);

                    if (updateDTO.UpdateCase != null && updateDTO.UpdateCase.CaseID == 0)
                    {
                        if (string.IsNullOrEmpty(updateDTO.UpdateCase.AccountNumber))
                        {
                            var coroClientDetail = CoRoHelper.GetClientDetailFromCoRoByEmail(updateDTO.UpdateEmail.Emailfrom, updateDTO.UpdateEmail.Country);

                            if (coroClientDetail != null)

                            {
                                updateDTO.UpdateCase.AccountNumber = coroClientDetail.AccountNumbers.Split(',').Length > 1 ? null : coroClientDetail.AccountNumbers;
                                updateDTO.UpdateCase.CIN = coroClientDetail.CIN ?? null;
                                updateDTO.UpdateCase.ClientName = coroClientDetail.ClientName ?? null;
                                updateDTO.UpdateCase.MultiAccountNumber = coroClientDetail.AccountNumbers ?? null;
                                updateDTO.UpdateCase.Country = coroClientDetail.Country ?? null;
                            }

                        }

                        updateDTO.UpdateCase.Country = updateDTO.UpdateCase.Country ?? updateDTO.UpdateEmail.Country;|
                        updateDTO.UpdateCase.FirstEmailID = email.EmailID;
                        updateDTO.UpdateCase.LastEmailID = email.EmailID;
                        updateDTO.UpdateCase.MailboxID = updateDTO.UpdateEmail.MailboxID;
                        updateDTO.UpdateCase.Priority = updateDTO.UpdateEmail.Priority;
                        updateDTO.UpdateCase.CurrentlyAssignedTo = updateDTO.CurrentUserId;
                        updateDTO.UpdateCase.CaseStatusID = (int)CaseStatus.CaseAssigned;
                        updateDTO.UpdateCase.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;
                        updateDTO.UpdateCase.CreatedBy = updateDTO.UpdateEmail.CurrentUserId;
                        updateDTO.UpdateCase.NewEmailCount = 0;

                        updateDTO.UpdateCase.KeepWithMe = true;

                        updateDTO = new CaseAPIController().SaveCaseDetails(updateDTO);
                    }
                    else
                    {

                        var CasedetailsUpdate = db.CaseDetails.First(x => x.CaseID == (int)updateDTO.UpdateCase.CaseID);
                        CasedetailsUpdate.LastEmailID = email.EmailID;
                        CasedetailsUpdate.LastViewedEmailID = email.EmailID;
                        CasedetailsUpdate.CaseStatusID = (CasedetailsUpdate.CaseStatusID == (int)CaseStatus.NewCase ? (int)CaseStatus.CaseAssigned : CasedetailsUpdate.CaseStatusID);// changing from new case to assigned
                        CasedetailsUpdate.NewEmailCount = ((CasedetailsUpdate.CaseIdIdentifer.Contains('#') == true || CasedetailsUpdate.CurrentlyAssignedTo != updateDTO.CurrentUserId) && updateDTO.UpdateEmail.EmailDirection.ToLower().Contains("close") == false ? (CasedetailsUpdate.NewEmailCount + 1) : 0);
                        CasedetailsUpdate.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;
                        CasedetailsUpdate.LastActedOn = DateTime.Now;

                        if (CasedetailsUpdate.IsCaseAcknowledged == null && updateDTO.UpdateCase.IsCaseAcknowledged == true)
                        {
                            CasedetailsUpdate.IsCaseAcknowledged = updateDTO.UpdateCase.IsCaseAcknowledged;
                            CasedetailsUpdate.CaseAckDateTime = DateTime.Now;
                            CasedetailsUpdate.AckMailId = email.EmailID;
                            CasedetailsUpdate.AckSentBy = email.CreatedBy;


                            db.Entry(CasedetailsUpdate).State = EntityState.Modified;
                            db.SaveChanges();

                            try
                            {
                                if (updateDTO.CustomData != null)
                                {
                                    var result = Task.Run(() => new CaseAllData().UpdateSubmittedAttributeByCaseId(updateDTO.UpdateCase.CaseID, updateDTO.CustomData)).Result;
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Info("exception in saving attribute");
                                logger.Info(ex.Message);
                                logger.Info(ex.StackTrace);
                                throw ex;
                            }

                        }

                        Tuple<List<string>, List<string>, string> Attachments = SaveAttachmentsToFolderAndDb(updateDTO.UpdateEmail.AttachmentTempFolder, updateDTO.UpdateEmail.ReceivedOn.Value, updateDTO.UpdateEmail.CaseID.Value, updateDTO.UpdateEmail.AttachmentPath,
                        email.EmailID, email.EmailBody, updateDTO.UpdateEmail.Country, null, "Outgoing");

                        var OutgoingEmailBody = Attachments.Item3;
                        List<string> inlineImagePaths = Attachments.Item1;
                        List<string> fileAttachmentsPath = Attachments.Item2;

                        if (updateDTO.Updatetmail.SaveAsDraft == false && !String.IsNullOrWhiteSpace(updateDTO.UpdateEmail.EmailTo))
                        {

                            SendEmail(email.EmailID, OutgoingEmailBody, updateDTO.UpdateEmail.CurrentUserId,
                            updateDTO.UpdateCase.CaseIdIdentifer, updateDTO.UpdateEmail.EmailTo, updateDTO.UpdateEmail.EmailCC, updateDTO.UpdateEmail.EmailBCC,
                            updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.Direction,
                            updateDTO.UpdateEmail.ListExistingFilesToRemoveForward, updateDTO.UpdateEmail.orginalEmailid, inlineImagePaths,
                            fileAttachmentsPath, updateDTO.UpdateEmail.UserName, updateDTO.UpdateCase.CategoryID,
                            updateDTO.UpdateEmail.EmailDirection, updateDTO.UpdateEmail.EmailClassificationLookup);
                        }

                        else

                        //TODO: Do something with email in this scenario

                        if (updateDTO.UpdateCase.CurrentlyAssignedTo == null)
                            updateDTO.UpdateCase = new CaseAllData().GetCaseByCaseID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.CaseID);
                        if (string.IsNull0rEmpty(Convert.ToString(updateDTO.UpdateCase.CurrentlyAssignedTo)))
                        {
                            updateDTO.UpdateCase.EmployeeName = null;
                        }
                        else
                        {
                            //case is assign to person by manually bulk assign who does not have access,code is breaking
                            //\With object reference error just avoid code breaking.
                            var employeeInfo = new UserData().GetUserInfoByEmployeeID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.CurrentlyAssignedTo);
                            if (employeeInfo != null)
                                updateDTO.UpdateCase.EmployeeName = employeeInfo.EmployeeName;
                            else
                                updateDTO.UpdateCase.EmployeeName = null;

                        }

                        if (updateDTO.UpdateCase.ParentCaseID != null && updateDTO.UpdateCase.ParentCaseID != 0 && updateDTO.UpdateCase.CaseIdIdentifer.Contains('#') == true)
                        {
                            CaseDetailVM ParentCase = new CaseAllData().GetCaseByCaseID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.ParentCaseID);
                            return new CaselEmailIdDTO
                            {
                                EmailId = email.EmaillD,
                                CaseId = updateDTO.UpdateEmail.CaseID,
                                CurrentlyAssignedTo = updateDTO.UpdateCase.CurrentlyAssignedTo,
                                EmployeeName = updateDTO.UpdateCase.EmployeeName,
                                CaseStatusID = updateDTO.UpdateCase.CaseStatusID,


                                CaseIdIdentifier = updateDTO.UpdateCase.CaseIdIdentifer,
                                ParentCaseAssignedTo = ParentCase.CurrentlyAssignedTo,
                                ParentCaseLastEmailld = (int)ParentCase.LastEmailID
                            };
                        }
                    }
                catch (Exception ex)
                {
                    //throw ex;
                    logger.Error("Exception Message in SaveNewOutboundEmailAndCreateCase:");
                    logger.Error(ex.Message);
                    logger.Error("Inner Exception:");
                    logger.Error(ex.InnerException);
                    logger.Error("Stack Trace:");
                    logger.Error(ex.StackTrace);
                    return new CaseIlEmailIdDTO { ErrorMessage = ex.Message };
                }
                return new CaselEmailldDTO
                {
                    Emailld = email.EmailID,
                    CaseIld = updateDTO.UpdateEmail.CaseID,
                    CurrentlyAssignedTo
                    updateDTO.UpdateCase.CurrentlyAssignedTo,
                    EmployeeName = updateDTO.UpdateCase.EmployeeName,
                    CaseStatusID
                    lipdateDTO.UpdateCase.CaseStatusID,
                    CaseIdIdentifier = updateDTO.UpdateCase.CaseIdIdentifer
                };



            }
        }


        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/SaveNewInboundEmailAndCreateCase")]
        public dynamic SaveNewInboundEmailAndCreateCase(CaseAndEmailUpdateDTO updateDTO)
        {
            //var request = Request;
            updateDTO.UpdateEmail.CurrentUserId = Environment.UserName;
            updateDTO.CurrentUserId = updateDTO.CurrentUserId ?? Environment.UserName;

            Tuple<int, string> newlyAddedCase = new Tuple<int, string>(0, null);
            string PartiallyMatchedToCases = "";
            CaseDetailVM caseDetailVM = new CaseDetailVM();
            Email email = null;

            string CaseIDIdentifier = "";
            // Save Email
            try
            {
                email = SaveEmail(updateDTO.UpdateEmail);
                if (email != null && email.EmailID != 0)
                {
                    updateDTO.UpdateEmail.EmailID = email.EmailID;

                    if (updateDTO.UpdateCase == null || updateDTO.UpdateCase.CaseID == 0)
                        updateDTO.UpdateCase = new CaseDetailVM();

                    if (updateDTO.UpdateEmail.CaseID == null)
                    {
                        updateDTO.UpdateEmail.CaseID = GetCaseIdFromEmailSubject(updateDTO.UpdateEmail.EmailSubject, updateDTO.UpdateEmail.Country, ref CaseIDIdentifier);
                        updateDTO.UpdateCase.CaseIdIdentifer = CaseIDIdentifier;

                        if (updateDTO.UpdateEmail.CaseID == null)
                        {
                            if (updateDTO.UpdateCase != null && updateDTO.UpdateCase.LastEmailID != null)
                            {
                                updateDTO.UpdateEmail.CaseID = Task.Run(() => new CaseAllData().GetCaseByLastEmailld(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.LastEmailID)).Result?.CaseID;
                            }
                            if (updateDTO.UpdateEmail.CaseID == null)
                            {

                                logger.Info("Partial match logic started");
                                var resultPartialMatch = EmailData.FindPartiallyMatchingCases(updateDTO.UpdateEmail.EmailSubject,
                                                        updateDTO.UpdateEmail.SentOn, updateDTO.UpdateEmail.ReceivedOn, email.TextBody, updateDTO.UpdateEmail.EmailTo,
                                                        updateDTO.UpdateEmail.EmailCC, updateDTO.UpdateEmail.Emailfrom, updateDTO.UpdateEmail.MailboxID,
                                                        updateDTO.UpdateEmail.Country);

                                if (resultPartialMatch != null && resultPartialMatch.Count == 1 && resultPartialMatch[@].CaseID != null)
                                {
                                    //attach to existing case
                                    updateDTO.UpdateEmail.CaseID = (int)resultPartialMatch[0].CaseID;
                                    logger.Info("Match Found:Caseid:" + updateDTO.UpdateEmail.CaseID);
                                }
                                else
                                {
                                    if (resultPartialMatch != null && resultPartialMatch.Count == 0)
                                        logger.Debug("No matching cases found going to create case");

                                    else if (resultPartialMatch != null && resultPartialMatch.Count > 1)
                                    {
                                        foreach (PartialCaseMatch pm in resultPartialMatch)
                                        {
                                            if (!PartiallyMatchedToCases.Contains(pm.CaseID.ToString()))
                                            {

                                                PartiallyMatchedToCases = PartiallyMatchedToCases + pm.CaseID.ToString() + ";";
                                            }
                                        }
                                        logger.Info("Partial subject match exception - Multiple matches found. Requires manual attachment");
                                    }

                                }

                                if (updateDTO.UpdateEmail.CaseID == null)
                                {
                                    if (updateDTO.UpdateCase == null || string.IsNullOrEmpty(updateDTO.UpdateCase.AccountNumber))
                                    {
                                        var coroClientDetail = CoRoHelper.GetClientDetailFromCoRoByEmail(updateDTO.UpdateEmail.EmailFrom, updateDTO.UpdateEmail.Country);

                                        if (coroClientDetail != null && coroClientDetail.AccountNumbers != null)
                                        {
                                            updateDTO.UpdateCase.AccountNumber = coroClientDetail.AccountNumbers.Split(',').Length > 1 ? null : coroClientDetail.AccountNumbers;
                                            updateDTO.UpdateCase.CIN = coroClientDetail.CIN ?? null;
                                            updateDTO.UpdateCase.ClientName = coroClientDetail.ClientName ?? null;
                                            updateDTO.UpdateCase.MultiAccountNumber = coroClientDetail.AccountNumbers ?? null;
                                            //updateDTO.UpdateCase.Country = coroClientDetail.Country >? null;

                                        }
                                    }

                                    updateDTO.UpdateCase.Country = updateDTO.UpdateCase.Country ?? updateDTO.UpdateEmail.Country;
                                    updateDTO.UpdateCase.FirstEmailID = email.EmailID;
                                    updateDTO.UpdateCase.LastEmailID = email.EmailID;
                                    updateDTO.UpdateCase.MailboxID = updateDTO.UpdateEmail.MailboxID;
                                    updateDTO.UpdateCase.Priority = updateDTO.UpdateEmail.Priority;
                                    // updateDTO.UpdateCase.CurrentlyAssignedTo = updateDTO.CurrentUserId;
                                    updateDTO.UpdateCase.IsCaseEscalated = updateDTO.UpdateEmail.IsCaseEscalated;
                                    updateDTO.UpdateCase.CreatedBy = updateDTO.UpdateEmail.CurrentUserId;
                                    updateDTO.UpdateCase.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;

                                }

                            }
                        }
                    }
                    if (updateDTO.UpdateEmail.CaseID != null)
                    {

                        CaseDetail CasedetailsUpdate;
                        bool isEscalated = false;
                        //var mailboxId = updateDTO.UpdateEmail.MailboxID;
                        //var Mailboxdetails = db.Mailboxes.First(x => x.MailboxID == (int)mailboxId);
                        //var Country = Mailboxdetails.Country;
                        using (BoltDbContext db = new BoltDbContext())
                        {
                            using (var transaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    CasedetailsUpdate = db.CaseDetails.First(x => x.CaseID == (int)updateDTO.UpdateEmail.CaseID);
                                    CasedetailsUpdate.NewEmailCount = CasedetailsUpdate.NewEmailCount + 1;
                                    if (!CasedetailsUpdate.IsCaseEscalated)
                                    {
                                        CasedetailsUpdate.IsCaseEscalated = updateDTO.UpdateEmail.IsCaseEscalated;
                                        if (updateDTO.UpdateEmail.IsCaseEscalated)
                                            isEscalated = true;
                                    }

                                    db.Entry(CasedetailsUpdate).State = EntityState.Modified;
                                    db.SaveChanges();
                                    transaction.Commit();
                                    new CaseAllData().InsertIntoCaseAuditTrailTable("An email has been received."
                                        + (isEscalated == true ? "Case is escalated." : ""), (int)updateDTO.UpdateEmail.CaseID, updateDTO.UpdateEmail.CurrentUserId, "email");
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback(); throw ex;
                                }
                            }
                        }

                        updateDTO.UpdateCase = updateDTO.UpdateCase = new CaseAllData().GetCaseByCaseID(CasedetailsUpdate.Country, updateDTO.UpdateEmail.CaseID);
                        updateDTO.UpdateCase.CaseID = (int)updateDTO.UpdateEmail.CaseID;
                        updateDTO.UpdateCase.LastEmailID = updateDTO.UpdateEmail.EmailID;
                        updateDTO.UpdateCase.LastActedOn = DateTime.Now;
                        updateDTO.UpdateCase.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;

                    }
                    updateDTO = new CaseAPIController().SaveCaseDetails(updateDTO);
                    new CaseAPIController().UpdateFiledCasesToAssigned();
                    Tuple<List<string>, List<string>, string> Attachments = SaveAttachmentsToFolderAndDb(updateDTO.UpdateEmail.AttachmentTempFolder,
                    updateDTO.UpdateEmail.ReceivedOn.Value, updateDTO.UpdateCase.CaseID, updateDTO.UpdateEmail.AttachmentPath, email.EmailID,
                    email.EmailBody, updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.savedAttachmentDetails, "Incoming");

                }

            }
            catch (Exception ex)
            {
                logger.Error("Exception Message in SaveNewInboundEmailAndCreateCase:");
                logger.Error(ex.Message);
                logger.Error("Inner Exception: ");
                logger.Error(ex.InnerException);
                logger.Error("Stack Trace:");
                logger.Error(ex.StackTrace);

                if (ex.Message == "Email with same hash is present")
                {
                    logger.Error("Inside email with same hash throwing to Email bot");
                    return new CaseIEmailIdDTO { EmailId = 0, CaseId = 0 };
                    //return Request.CreateResponse<string>(HttpStatusCode.NotAcceptable, "Email with same hash is present");// Default message if exception occured
                }
                CleanupEmailTraces(email.EmailID);
                throw ex;
            }
            return new CaseIEmailIdDTO
            {
                Emailld = email.EmailID,
                CaseId = updateDTO.UpdateEmail.CaseID,
                CaseIdIdentifier = updateDTO.UpdateCase.CaseIdIdentifer
            };

        }
    }
}