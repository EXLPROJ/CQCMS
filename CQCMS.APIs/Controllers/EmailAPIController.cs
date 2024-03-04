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
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;

namespace CQCMS.API.Controllers
{
    public class EmailAPIController : ApiController
    {
        private static Logger logger = LogManager.GetLogger("EmailTransformation");
        // GET: EmailAPI
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/SaveNewEmailAndCreateCase")]
        public dynamic SaveNewEmailAndCreateCase(CaseAndEmailUpdateDTO updateDTO)
        {
            CaseIdEmailIdDTO CaseIEmailIdDTO;
            if (updateDTO.UpdateEmail.Direction == "Incoming")
                CaseIEmailIdDTO = SaveNewInboundEmailAndCreateCase(updateDTO);
            else
                CaseIEmailIdDTO = SaveNewOutboundEmailAndCreateCase(updateDTO);
            return CaseIEmailIdDTO;

        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/SaveNewOutboundEmailAndCreateCase")]
        public dynamic SaveNewOutboundEmailAndCreateCase(CaseAndEmailUpdateDTO updateDTO)
        {

            using (CQCMSDbContext db = new CQCMSDbContext())
            {

                CaseIdEmailIdDTO CaseIdEmailIdDTO;
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
                            //var coroClientDetail = CoRoHelper.GetClientDetailFromCoRoByEmail(updateDTO.UpdateEmail.EmailFrom, updateDTO.UpdateEmail.Country);

                            //if (coroClientDetail != null)

                            //{
                            //    updateDTO.UpdateCase.AccountNumber = coroClientDetail.AccountNumbers.Split(',').Length > 1 ? null : coroClientDetail.AccountNumbers;
                            //    updateDTO.UpdateCase.CIN = coroClientDetail.CIN ?? null;
                            //    updateDTO.UpdateCase.ClientName = coroClientDetail.ClientName ?? null;
                            //    updateDTO.UpdateCase.MultiAccountNumber = coroClientDetail.AccountNumbers ?? null;
                            //    updateDTO.UpdateCase.Country = coroClientDetail.Country ?? null;
                            //}

                        }

                        updateDTO.UpdateCase.Country = updateDTO.UpdateCase.Country ?? updateDTO.UpdateEmail.Country;
                        updateDTO.UpdateCase.FirstEmailID = email.EmailID;
                        updateDTO.UpdateCase.LastEmailID = email.EmailID;
                        updateDTO.UpdateCase.MailboxID = updateDTO.UpdateEmail.MailboxID;
                        updateDTO.UpdateCase.Priority = updateDTO.UpdateEmail.Priority;
                        updateDTO.UpdateCase.CurrentlyAssignedTo = updateDTO.CurrentUserId;
                        updateDTO.UpdateCase.CaseStatusID = (int)CaseStatus.CaseAssigned;
                        updateDTO.UpdateCase.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;
                        updateDTO.UpdateCase.CreatedBy = updateDTO.UpdateEmail.CurrentUserId;
                        updateDTO.UpdateCase.NewEmailcount = 0;

                        updateDTO.UpdateCase.KeepWithMe = true;

                        //updateDTO = new CaseAPIController().SaveCaseDetails(updateDTO);
                    }
                    else
                    {

                        var CasedetailsUpdate = db.CaseDetails.First(x => x.CaseID == (int)updateDTO.UpdateCase.CaseID);
                        CasedetailsUpdate.LastEmailID = email.EmailID;
                        CasedetailsUpdate.LastViewedEmailID = email.EmailID;
                        CasedetailsUpdate.CaseStatusID = (CasedetailsUpdate.CaseStatusID == (int)CaseStatus.NewCase ? (int)CaseStatus.CaseAssigned : CasedetailsUpdate.CaseStatusID);// changing from new case to assigned
                        CasedetailsUpdate.NewEmailCount = ((CasedetailsUpdate.CaseIdIdentifier.Contains('#') == true || CasedetailsUpdate.CurrentlyAssignedTo != updateDTO.CurrentUserId) && updateDTO.UpdateEmail.EmailDirection.ToLower().Contains("close") == false ? (CasedetailsUpdate.NewEmailCount + 1) : 0);
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

                        if (updateDTO.UpdateEmail.SaveAsDraft == false && !String.IsNullOrWhiteSpace(updateDTO.UpdateEmail.EmailTo))
                        {

                            //SendEmail(email.EmailID, OutgoingEmailBody, updateDTO.UpdateEmail.CurrentUserId,
                            //updateDTO.UpdateCase.CaseIdIdentifier, updateDTO.UpdateEmail.EmailTo, updateDTO.UpdateEmail.EmailCC, updateDTO.UpdateEmail.EmailBCC,
                            //updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.Direction,
                            //updateDTO.UpdateEmail.ListExistingFilesToRemoveForward, updateDTO.UpdateEmail.orginalEmailid, inlineImagePaths,
                            //fileAttachmentsPath, updateDTO.UpdateEmail.UserName, updateDTO.UpdateCase.CategoryID,
                            //updateDTO.UpdateEmail.EmailDirection, updateDTO.UpdateEmail.EmailClassificationLookup);
                        }

                        else

                        //TODO: Do something with email in this scenario

                        if (updateDTO.UpdateCase.CurrentlyAssignedTo == null)
                            updateDTO.UpdateCase = new CaseAllData().GetCaseByCaseID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.CaseID);
                        if (string.IsNullOrEmpty(Convert.ToString(updateDTO.UpdateCase.CurrentlyAssignedTo)))
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

                        if (updateDTO.UpdateCase.ParentCaseID != null && updateDTO.UpdateCase.ParentCaseID != 0 && updateDTO.UpdateCase.CaseIdIdentifier.Contains('#') == true)
                        {
                            CaseDetailVM ParentCase = new CaseAllData().GetCaseByCaseID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.ParentCaseID);
                            return new CaseIdEmailIdDTO
                            {
                                EmailId = email.EmailID,
                                CaseId = updateDTO.UpdateEmail.CaseID,
                                CurrentlyAssignedTo = updateDTO.UpdateCase.CurrentlyAssignedTo,
                                EmployeeName = updateDTO.UpdateCase.EmployeeName,
                                CaseStatusID = updateDTO.UpdateCase.CaseStatusID,


                                CaseIdIdentifier = updateDTO.UpdateCase.CaseIdIdentifier,
                                ParentCaseAssignedTo = ParentCase.CurrentlyAssignedTo,
                                ParentCaseLastEmailId = (int)ParentCase.LastEmailID
                            };
                        }
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
                    return new CaseIdEmailIdDTO { ErrorMessage = ex.Message };
                }
                return new CaseIdEmailIdDTO
                {
                    EmailId = email.EmailID,
                    CaseId = updateDTO.UpdateEmail.CaseID,
                    CurrentlyAssignedTo = updateDTO.UpdateCase.CurrentlyAssignedTo,
                    EmployeeName = updateDTO.UpdateCase.EmployeeName,
                    CaseStatusID = updateDTO.UpdateCase.CaseStatusID,
                    CaseIdIdentifier = updateDTO.UpdateCase.CaseIdIdentifier
                };

            }
        }

        //[System.Web.Http.AllowAnonymous]
        //[System.Web.Http.HttpPost]
        //[System.Web.Http.Route("api /SaveNewOutboundEmailAndCreateCase”) ]
        //public dynamic SaveNlewWutboundEmailAndCreateCase(CaseAndEmailUpdateDTO updateDTO)
        //{

        //    using (CQCMSDbContext db = new CQCMSDbContext())
        //    {
        //        CaseIEmailIdDT0 CaseTEmailIdDT0;
        //        Email email = null;|
        //        var currentUser = new UserData().GetUserInfoByEmployeeID(updateDTO.UpdateEmail.Country, updateDTO.CurrentUserId);
        //        var mailboxaddress = new MailboxData().GetMailboxbyID(updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.MailboxID).MailboxNlame;

        //        if (currentUser != null && currentUser.EmployeeName! && mailboxaddress != null && mailboxaddress != "")
        //        {
        //            updateDTO.UpdateEmail.EmailFrom = currentUser.EmployeeName + " <" + mailboxaddress + ">";

        //        }
        //        try
        //        {

        //            email = SaveEmail(updateDTO.UpdateEmail);

        //            if (updateDT0.UpdateCase != null && updateDTO.UpdateCase.CaseID == 0)

        //            {
        //                if (string.IsNul10rEmpty(updateDTO.UpdateCase.AccountNumber))
        //                {
        //                    var coroClientDetail = CoRoHelper.GetClientDetailFromCoRoByEmail(updateDTO.UpdateEmail.£mailFrom, updateDTO.UpdateEmail.Country);
        //                    if (coroClientDetail != null)
        //                    {
        //                        updateDTO.UpdateCase.AccountNumber = coroClientDetail.AccountNumbers.Split(',').Length > 1 ? null : coroClientDetail.Accountnumber; koroClientDetail.AccountNumbers;
        //                        updateDTO.UpdateCase.CIN = coroClientDetail.CIN ?? null;
        //                        updateDT0.UpdateCase.ClientName = coroClientDetail.ClientName ?? null;
        //                        updateDTO.UpdateCase.MultiAccountNumber = coroClientDetail.AccountNumbers ?? null;
        //                        updateDTO.UpdateCase.Country = coroClientDetail.Country ?? null;
        //                    }
        //                }

        //                updateDTO.UpdateCase.Country = updateDTO.UpdateCase.Country ?? updateDTO.UpdateEmail.Country;
        //                updateDTO.UpdateCase.FirstEmailID = email.EmailID;
        //                updateDTO.UpdateCase.LastEmailID = email.EmailID;
        //                updateDTO.UpdateCase.MailboxID = updateDTO.UpdateEmail.MailboxID;
        //                updateDTO.UpdateCase.Priority = updateDTO.UpdateEmail.Priority;
        //                updateDTO.UpdateCase.CurrentlyAssignedTo = updateDTO.CurrentUserId;
        //                updateDTO.UpdateCase.CaseStatusID = (int)CaseStatus.CaseAssigned;
        //                updateDTO.UpdateCase.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;
        //                updateDTO.UpdateCase.CreatedBy = updateDTO.UpdateEmail.CurrentUserId;
        //                updateDTO.UpdateCase.NewEmailCount = 0;

        //                updateDTO.UpdateCase.KeepWithMe = true;

        //                updateDTO = new CaseAPIController().SaveCaseDetails(updateDTO);

        //            }
        //            else
        //            {
        //                var CasedetailsUpdate = db.CaseDetails.First(x => x.CaseID == (int)updateDTO.UpdateCase.CaseID)
        //                CasedetailsUpdate.LastEmailID = email.EmailID;
        //                CasedetailsUpdate.LastViewedEmailID = email.EmailID;
        //                CasedetailsUpdate.CaseStatusID = (CasedetailsUpdate.CaseStatusID == (int)CaseStatus.NewCase ? (int)CaseStatus.CaseAssigned : CasedetailsUpdate.CaseStatusID);// changing from new case to assigned

        //                CasedetailsUpdate.NewEmailCount = ((CasedetailsUpdate.CaseIdIdentifier.Contains("#") == true || CasedetailsUpdate.CurrentlyAssignedTo != updateDTO.CurrentUserId) && updateDTO.UpdateEmail.EmailDirection.ToLower().Contains("close") == false ? (CasedetailsUpdate.NewEmailCount + 1) : 0);
        //                CasedetailsUpdate.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;

        //                CasedetailsUpdate.LastActedOn = DateTime.Now;



        //                if (CasedetailsUpdate.IsCaseAcknowledged == null && updateDTO.UpdateCase.IsCaseAcknowledged == true)
        //                {



        //                    CasedetailsUpdate.IsCaseAcknowledged = updateDTO.UpdateCase.IsCaseAcknowledged;
        //                    CasedetailsUpdate.CaseAckDateTime = DateTime.Now;

        //                    CasedetailsUpdate.AckMailId = email.EmailID;

        //                    CasedetailsUpdate.AckSentBy = email.CreatedBy;



        //                    db.Entry(CasedetailsUpdate).State = EntityState.Modified;
        //                    db.SaveChanges();

        //                    try
        //                    {

        //                        if (updateDTO.CustomData != null)
        //                        {



        //                            var result = Task.Run(() => new CaseAllData().UpdateSubmittedAttributeByCaseld(updateDTO.UpdateCase.CaseID, updateDTO.CustomData)).Result;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        logger.Info("exception in saving attribute");
        //                        logger.Info(ex.Message);
        //                        logger.Info(ex.StackTrace);
        //                        throw ex;
        //                    }
        //                }

        //                Tuple<List<string>, List<string>, string> Attachments = SaveAttachmentsToFolderAndDb(updateDTO.UpdateEmail.AttachmentTempFolder,
        //                updateDTO.UpdateEmail.ReceivedOn.Value, updateDTO.UpdateEmail.CaseID.Value, updateDTO.UpdateEmail.AttachmentPath, email.EmailID, email.EmailBody, updateDTO.UpdateEmail.Country, null, "Outgoing");
        //                var OutgoingEmailBody = Attachments.Item3;
        //                List<string> inlineImagePaths = Attachments.Item1;
        //                List<string> fileAttachmentsPath = Attachments.Item2;

        //                if (updateDT0.UpdateEmail.SaveAsDraft == false && !String.IsNullOrWhiteSpace(updateDTO.UpdateEmail.EmailTo))
        //                {



        //                    SendEmail(email.EmailID, OutgoingEmailBody, updateDTO.UpdateEmail.CurrentUserId, updateDTO.UpdateCase.CaseIdIdentifier,
        //                        updateDTO.UpdateEmail.EmailTo, updateDTO.UpdateEmail.EmailCC,
        //                    updateDTO.UpdateEmail.EmailBCC, updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.Direction,
        //                    updateDTO.UpdateEmail.ListExistingFilesToRemoveForward, updateDTO.UpdateEmail.orginalEmailid, inlineImagePaths,
        //                    fileAttachmentsPath, updateDTO.UpdateEmail.UserName, updateDTO.UpdateCase.CategoryID, updateDTO.UpdateEmail.EmailDirection,
        //                    updateDTO.UpdateEmail.EmailClassificationLookup);
        //                }

        //                else

        //                {


        //                    //T0D0: Do something with email in this scenario
        //                }

        //                if (updateDTO.UpdateCase.CurrentlyAssignedTo == null)
        //                {

        //                    updateDTO.UpdateCase = new CaseAllData().GetCaseByCaseID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.CaseID);
        //                    if (string.IsNullOrEmpty(Convert.ToString(updateDTO.UpdateCase.CurrentlyAssignedTo)))
        //                    {

        //                        updateDTO.UpdateCase.EmployeeName = null;
        //                    }


        //                    else

        //                    {
        //                        //case is assign to person by manually bulk assign who does not have access,code is breaking with object referenceerror just avoid code breaking.

        //                        var employeeInfo = new UserData().GetUserInfoByEmployeeID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.CurrentlyAssignedTo);
        //                        if (employeeInfo != null)
        //                            updateDTO.UpdateCase.EmployeeName = employeeInfo.EmployeeName;
        //                        else
        //                            updateDTO.UpdateCase.EmployeeName = null;
        //                    }
        //                }

        //                if (updateDTO.UpdateCase.ParentCaseID != null && updateDTO.UpdateCase.ParentCaseID != 0 && updateDTO.UpdateCase.CaseIdIdentifier.Contains("#") == true)
        //                {

        //                    CaseDetailVM ParentCase = new CaseAllData().GetCaseByCaseID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.ParentCaseID);
        //                    return new CaseIEmailIdDTO

        //                    {
        //                        Emailld = email.EmailID,
        //                        CaseId = updateDTO.UpdateEmail.CaseID,
        //                        CurrentlyAssignedTo = updateDTO.UpdateCase.CurrentlyAssignedTo,
        //                        EmployeeName = updateDTO.UpdateCase.EmployeeName,
        //                        CaseStatusID = updateDTO.UpdateCase.CaseStatusID,
        //                        CaseldIdentifier = updateDTO.UpdateCase.CaseIdIdentifier,
        //                        ParentCaseAssignedTo = ParentCase.CurrentlyAssignedTo,
        //                        ParentCaseLastEmailld = (int)ParentCase.LastEmailID
        //                    };
        //                }
        //            }
        //                catch (Exception ex)
        //        {
        //            //throw ex;
        //            logger.Error("Exception Message in SaveNlew0utboundEmailAndCreateCase:");
        //            logger.Error(ex.Message);
        //            logger.Error("Inner Exception: ");

        //            logger.Error(ex.InnerException);
        //            logger.Error("Stack Trace:");
        //            logger.Error(ex.StackTrace);
        //            return new CaseIEmailldDT0 { ErrorMessage = ex.Message };
        //        }


        //        return new CaseIEmailIdDT0 { Emailld = email.EmailID, CaseId = updateDTO.UpdateEmail.CaseID, CurrentlyAssignedTo = updateDTO.UpdateCase.CurrentlyAssignedTo,
        //            Employeelame = updateDTO.UpdateCase.EmployeeName, CaseStatusID= updateDTO.UpdateCase.CaseStatusID, CaseIdIdentifier = updateDTO.UpdateCase.CaseIdIdentifier };
        //    }
        //}

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
                        updateDTO.UpdateCase.CaseIdIdentifier = CaseIDIdentifier;

                        if (updateDTO.UpdateEmail.CaseID == null)
                        {
                            if (updateDTO.UpdateCase != null && updateDTO.UpdateCase.LastEmailID != null)
                            {
                                updateDTO.UpdateEmail.CaseID = Task.Run(() => new CaseAllData().GetCaseByLastEmailId(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.LastEmailID)).Result?.CaseID;
                            }
                            if (updateDTO.UpdateEmail.CaseID == null)
                            {

                                logger.Info("Partial match logic started");
                                var resultPartialMatch = EmailData.FindPartiallyMatchingCases(updateDTO.UpdateEmail.EmailSubject,
                                                        updateDTO.UpdateEmail.SentOn, updateDTO.UpdateEmail.ReceivedOn, email.TextBody, updateDTO.UpdateEmail.EmailTo,
                                                        updateDTO.UpdateEmail.EmailCC, updateDTO.UpdateEmail.EmailFrom, updateDTO.UpdateEmail.MailboxID,
                                                        updateDTO.UpdateEmail.Country);

                                if (resultPartialMatch != null && resultPartialMatch.Count == 1 && resultPartialMatch[0].CaseID != null)
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
                                    //if (updateDTO.UpdateCase == null || string.IsNullOrEmpty(updateDTO.UpdateCase.AccountNumber))
                                    //{
                                    //    var coroClientDetail = CoRoHelper.GetClientDetailFromCoRoByEmail(updateDTO.UpdateEmail.EmailFrom, updateDTO.UpdateEmail.Country);

                                    //    if (coroClientDetail != null && coroClientDetail.AccountNumbers != null)
                                    //    {
                                    //        updateDTO.UpdateCase.AccountNumber = coroClientDetail.AccountNumbers.Split(',').Length > 1 ? null : coroClientDetail.AccountNumbers;
                                    //        updateDTO.UpdateCase.CIN = coroClientDetail.CIN ?? null;
                                    //        updateDTO.UpdateCase.ClientName = coroClientDetail.ClientName ?? null;
                                    //        updateDTO.UpdateCase.MultiAccountNumber = coroClientDetail.AccountNumbers ?? null;
                                    //        //updateDTO.UpdateCase.Country = coroClientDetail.Country >? null;

                                    //    }
                                    //}

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
                        using (CQCMSDbContext db = new CQCMSDbContext())
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

                                    db.Entry(CasedetailsUpdate).State = System.Data.Entity.EntityState.Modified;
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
                    return new CaseIdEmailIdDTO { EmailId = 0, CaseId = 0 };
                    //return Request.CreateResponse<string>(HttpStatusCode.NotAcceptable, "Email with same hash is present");// Default message if exception occured
                }
                CleanupEmailTraces(email.EmailID);
                throw ex;
            }
            return new CaseIdEmailIdDTO
            {
                EmailId = email.EmailID,
                CaseId = updateDTO.UpdateEmail.CaseID,
                CaseIdIdentifier = updateDTO.UpdateCase.CaseIdIdentifier
            };

        }


        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/SaveEmail")]
        public dynamic SaveEmail(EmailVM UpdateEmail)
        {
            var mailHash = "";
            Email email = new Email();

            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (UpdateEmail.EmailSubject == null)
                        {
                            logger.Info("Email subject is empty");
                            throw new Exception("Email subject is empty");
                        }
                        else
                        {
                            logger.Info(UpdateEmail.EmailSubject);
                        }

                        Regex tagSingleRegex = new Regex("<([*>]+)>");
                        if (tagSingleRegex.IsMatch(UpdateEmail.EmailSubject))
                        {
                            UpdateEmail.EmailSubject = tagSingleRegex.Replace(UpdateEmail.EmailSubject, "[$1]");
                        }
                        if (UpdateEmail.EmailBody == null)
                        {
                            logger.Info("Email body was null, will be saved as blank");
                            UpdateEmail.EmailBody = "";
                        }
                        else
                        {
                            try
                            {
                                //UpdateEmail.TextBody = ExchangeEngine.ConvertHtm1BodyToTextBody(UpdateEmail.EmailBody);
                            }
                            catch (Exception ex)
                            {
                                logger.Info("Error in converting email body to text body" + ex.Message);
                                throw ex;
                            }
                        }
                        if (UpdateEmail.Direction.ToLower() == "incoming")
                        {

                            mailHash = GetMailHash(UpdateEmail.EmailFrom, UpdateEmail.EmailTo, UpdateEmail.EmailSubject, UpdateEmail.TextBody,
                                        UpdateEmail.SentOn.HasValue ? UpdateEmail.SentOn.Value.ToString("dd-MMM-yyyy HH:mm:ss") : "");
                            int hashMatchResult = EmailData.IsEmailPresentWithHash(UpdateEmail.CaseID.HasValue ? UpdateEmail.CaseID.Value.ToString() : "0",
                                        mailHash, UpdateEmail.SentOn.HasValue ? UpdateEmail.SentOn.Value.ToString("dd-MMM-yyyy HH:mm:ss") : "", UpdateEmail.Country);
                            if (hashMatchResult != 0)
                            {

                                try
                                {
                                    logger.Info("Email with same hash is present");
                                    throw new Exception("Email with same hash is present");
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Email with same hash is present");
                                    logger.Error("Exception gccured moving duplicate mail with hash " + mailHash + " to processed. Error message " +
                                    ex.Message + Environment.NewLine + ex.StackTrace);
                                }
                            }
                        }

                        if ((UpdateEmail.SaveAsDraft || UpdateEmail.Direction == "Outgoing") && !UpdateEmail.EmailSubject.ToLower().Contains("draft"))
                        {
                            UpdateEmail.EmailSubject = "DRAFT: " + UpdateEmail.EmailSubject;
                        }
                        logger.Info("Creating Email");

                        email = new Email
                        {
                            EmailID = UpdateEmail.orginalEmailid,
                            CaseID = UpdateEmail.CaseID,
                            MailboxID = UpdateEmail.MailboxID, //remove this field from case table
                            Emailstatus = UpdateEmail.EmailStatus,
                            Emailto = UpdateEmail.EmailTo,
                            Emailcc = UpdateEmail.EmailCC != null ? UpdateEmail.EmailCC : "",
                            EmailBcc = UpdateEmail.BccReceipients != null ? UpdateEmail.BccReceipients : "",
                            EmailBody = UpdateEmail.EmailBody,
                            EmailFolder = UpdateEmail.EmailFolder,
                            EmailFrom = UpdateEmail.EmailFrom,
                            ReceivedOn = UpdateEmail.ReceivedOn,
                            EmailSubject = UpdateEmail.EmailSubject,
                            TextBody = UpdateEmail.TextBody,
                            LastActedOn = DateTime.Now,
                            Createdon = DateTime.Now,
                            CreatedBy = UpdateEmail.CurrentUserId,
                            LastActedBy = UpdateEmail.CurrentUserId,
                            EmailDirection = UpdateEmail.Direction,
                            Priority = UpdateEmail.Priority,
                            Country = UpdateEmail.Country,
                            EmailTrimmedSubject = new EmailData().CleanEmailSubject(UpdateEmail.EmailSubject.Replace("'", "''")),
                            //EmailHash = mailHash
                        };
                        if (UpdateEmail.EmailBody != "" && UpdateEmail.EmailBody.Contains("BEGIN VOLTAGE SECURE BLOCK") && UpdateEmail.EmailBody.Contains("END VOLTAGE SECURE BLOCK"))
                        {
                            int indexofvoltageSecureStart = UpdateEmail.EmailBody.IndexOf("BEGIN VOLTAGE SECURE BLOCK");
                            int indexofvoltageSecureEnd = UpdateEmail.EmailBody.LastIndexOf("END VOLTAGE SECURE BLOCK");

                            string strippedBodyText = UpdateEmail.EmailBody.Remove((indexofvoltageSecureStart - 35), ((indexofvoltageSecureEnd - 38)
                                    - (indexofvoltageSecureStart - 38)));

                            //Microsoft.Exchange.WebServices.Data.MessageBody strippedBody = (Microsoft.Exchange.WebServices.Data.MessageBody)strippedBodyText;
                            email.EmailBody = strippedBodyText;

                            if (UpdateEmail.CaseID == null)
                                email.EmailtypeID = 1; //initial email
                            else
                                email.EmailtypeID = 2; //incoming email, but not first email, typeid 2 is email reply
                            try
                            {
                                db.Emails.Add(email);
                                db.SaveChanges();
                                transaction.Commit();
                                logger.Info("Email Created, Emailid:" + email.EmailID);

                            }
                            catch (Exception ex)
                            {
                                logger.Info("Exception occured saving new email to DB: " + ex.StackTrace);
                                throw new Exception("Error saving email to db");
                            }
                            Console.WriteLine(Environment.NewLine + "Email with subject:" + email.EmailSubject + " saved" + Environment.NewLine);
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Exception occured: " + ex);
                        transaction.Rollback();
                    }
                }
            }

            return email;
        }

        public static string GetMailHash(string From, string ToRecipients, string Subject, string TextBody, string SentOn)
        {
            // xemove name from recipients (only use email id)
            // remove fy: and re: and [e] from subject and trim
            // txim everything and make everything lower

            string content = String.Join(";", PrepareReceipients(From)) + "|" +
                String.Join(";", PrepareReceipients(ToRecipients)) + "|" +
        String.Join("", Subject.ToLower().Split(new string[] { "re:", "fw:", "[e]" }, StringSplitOptions.RemoveEmptyEntries)).Trim() + "|" +
        String.Join("", TextBody.ToLower().Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));


            StringBuilder Sb = new StringBuilder();
            //logger.Debug ("Content pre-hash: " + content);
            using (SHA256 hash = SHA256Managed.Create())

            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(content));
                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        public static List<string> PrepareReceipients(string Receipient)
        {
            List<string> allReceipients = new List<string>();
            try
            {
                allReceipients = Receipient.Split(new char[] { ',', ';', ';' }, StringSplitOptions.RemoveEmptyEntries).Where(email =>
                            email.Contains("@")).ToList();
                allReceipients = allReceipients.Select(email => (email.IndexOf("<") == -1 ? email.Replace('{', '<').Replace(')', '>').Replace('}', '>') : email)).ToList();
                allReceipients = allReceipients.Select(email => (email.Contains("<") ? email.Split(new char[] { '<', '>' })[1] : email).Trim().ToLower())
                                    .AsQueryable<string>().Select(m => m.Trim()).ToList();
                if (allReceipients.Any(e => e.Count(x => x == '@') > 1))
                    throw new Exception("Invalid recipients");
            }
            catch (Exception ex)
            {
                logger.Debug("Error cleaning up recipients");
            }
            return allReceipients;
        }

        public static Tuple<List<string>, List<string>, string> SaveAttachmentsToFolderAndDb(string AttachmentTempFolder, DateTime ReceivedOn, int CaseId, string attachmentPath,

        int EmailId, string OutgoingEmailBody, string Country, List<SavedAttachment> savedAttachmentDetails = null, string Direction = null)

        {

            List<string> inlineImagePaths = new List<string>();

            List<string> fileAttachmentsPath = new List<string>();

            string destinationPath;

            try

            {

                destinationPath = Path.Combine(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Attachments"), DateTime.Today.ToString(@"yyyy\\MM\\da")), CaseId.ToString());

                if (!string.IsNullOrEmpty(attachmentPath))

                {

                    List<FileInfo> MultipleFiles = new List<FileInfo>();

                    foreach (var filepath in attachmentPath.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries))

                    {

                        string tempfilepath = filepath;

                        //Wnen FWD email with existing attachment(s)

                        if (filepath.IndexOf("fileName") >= 0)

                        {

                            tempfilepath = Path.Combine("/Attachments/", filepath.Split(new string[] { "fileName=" }, StringSplitOptions.RemoveEmptyEntries)[1]);

                        }

                        MultipleFiles.Add(new FileInfo(System.Web.HttpContext.Current.Server.MapPath("-" + System.Web.HttpUtility.UrlDecode(tempfilepath))));

                    }

                    logger.Info("Retrieved total number of attachments: " + MultipleFiles.Count());

                    fileAttachmentsPath = SaveAttachments(MultipleFiles.ToArray(), destinationPath, EmailId, CaseId, Country, savedAttachmentDetails, Direction);

                    logger.Info("Saved attachments to the database for EmailID: " + EmailId);

                }

                else

                {

                    logger.Info("There is no attachment in the email for EmailID: " + EmailId);

                }

            }

            catch (Exception ex)

            {

                logger.Info("Exception ggcured new case: Mapping path to the file attachment, Direction of the email: " + Direction);

                logger.Info(ex.Message);

                logger.Info(ex.StackTrace);

                if (Direction == "Incoming")

                {

                    logger.Debug("Before cleaning up email traces for email id: " + EmailId);

                    new EmailData().CleanupEmailTraces(EmailId);

                }

                throw ex;

            }

            try
            {
                using (CQCMSDbContext db = new CQCMSDbContext())
                {
                    var rootUrl = ConfigData.GetConfigValue("rootUri");

                    //for sgessnshoyg in email body..replace img tag azg--

                    Email updateEmailImgTag = db.Emails.Where(x => x.EmailID == EmailId).FirstOrDefault();

                    if (updateEmailImgTag != null)
                    {
                        string emailbodywithUpdatediImgTags = updateEmailImgTag.EmailBody;
                        string rex = @"[<][i][m][g][^>]*[>]";
                        string Srcregex = "[s][r][c][=][\"](^\"]*[\"]";
                        var allImgtags = Regex.Matches(emailbodywithUpdatediImgTags, rex);

                        //iogger Info ("Found total images : " + allimgtags.Count);

                        foreach (var img in allImgtags)
                        {
                            var srcvalue = Regex.Match(img.ToString(), Srcregex).ToString().Replace("src=", "").Replace("\"", "");
                            //logger Info ("image source - " + azcvalue)

                            if (srcvalue.StartsWith("cid"))
                            {
                                var matchingAttachment = fileAttachmentsPath.FirstOrDefault(a => a.Contains(srcvalue.Substring(4)));
                                if (matchingAttachment != null)
                                    matchingAttachment = matchingAttachment.ToLower().Contains("attachments") ? matchingAttachment.Split(new string[] { "Attachments" }, StringSplitOptions.RemoveEmptyEntries)[1].TrimStart(new char[] { '/', '\\' }) : matchingAttachment;

                                if (!string.IsNullOrWhiteSpace(matchingAttachment))
                                    emailbodywithUpdatediImgTags = emailbodywithUpdatediImgTags.Replace(srcvalue, Path.Combine(@"..\attachments", matchingAttachment));
                            }

                            if (srcvalue.StartsWith("data:image"))
                            {
                                var base64 = srcvalue.Substring(srcvalue.IndexOf(";base64,")).Replace(";base64,", "");
                                var extension = Regex.Match(srcvalue, "[/][^;]*[;]").ToString().Replace("/", "").Replace(";", "");

                                try
                                {
                                    var savedFilePath = WebHelperMethods.Savebase64image(base64, extension, destinationPath);
                                    inlineImagePaths.Add(savedFilePath);
                                    OutgoingEmailBody = OutgoingEmailBody.Replace(srcvalue, "cid:" + Path.GetFileName(savedFilePath));
                                    emailbodywithUpdatediImgTags = emailbodywithUpdatediImgTags.Replace(srcvalue, "..\\" + savedFilePath.Substring(savedFilePath.ToLower().IndexOf("attachments")));

                                }

                                catch (Exception ex)
                                {
                                    logger.Info("Not able to replace base64 image  with string: " + base64);
                                    logger.Info("Error in replace inline image: " + ex.Message);
                                }

                            }
                            else if (srcvalue.StartsWith("..") || srcvalue.StartsWith(rootUrl, StringComparison.OrdinalIgnoreCase))
                            {
                                inlineImagePaths.Add(System.Web.HttpContext.Current.Server.MapPath("~\\" + srcvalue.Substring(srcvalue.ToLower().IndexOf("atcachments"))));
                                OutgoingEmailBody = OutgoingEmailBody.Replace(srcvalue, "cid" + Path.GetFileName(srcvalue));
                            }

                        }

                        var emailToUpdate = db.Emails.First(x => x.EmailID == EmailId);
                        emailToUpdate.EmailBody = emailbodywithUpdatediImgTags;
                        db.Entry(emailToUpdate).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                }

            }

            catch (Exception ex)
            {
                logger.Info("Exception gggused new case: saving file attachment block and replacing agzssagkgki, Direction of the email: " + Direction);
                logger.Info(ex.Message);
                logger.Info(ex.StackTrace);

                if (Direction == "Incoming")
                {
                    logger.Debug("Before cleaning up email traces for email id: " + EmailId);
                    new EmailData().CleanupEmailTraces(EmailId);
                }

                throw ex;

            }
            return new Tuple<List<string>, List<string>, string>(inlineImagePaths, fileAttachmentsPath, OutgoingEmailBody);

        }

        public static List<string> SaveAttachments(FileInfo[] MultipleFiles, string DestinationPath, int EmailId, int CaseId, string Country, List<SavedAttachment> savedAttachmentDetails = null, string Direction = null)
        {
            List<string> allAttachedFiles = new List<string>();

            if (MultipleFiles.Length != 0)
            {
                for (int i = 0; i < MultipleFiles.Length; i++)
                {
                    try
                    {
                        FileInfo FileUpload = MultipleFiles[i];
                        if (FileUpload.Name.Length > 0)
                        {
                            string _path = "";
                            string _FileName = Path.GetFileName(FileUpload.Name);

                            if (FileUpload.FullName.ToLower().Contains("temporaryattachments"))
                            {
                                _path = Path.Combine(DestinationPath, Path.GetFileNameWithoutExtension(_FileName) + FileUpload.Extension);
                                if (System.IO.File.Exists(_path))
                                    _path = Path.Combine(DestinationPath, Path.GetFileNameWithoutExtension(_FileName) + "_" + (DateTime.Now.Ticks - DateTime.Today.Ticks) + FileUpload.Extension);

                                else if (!Directory.Exists(DestinationPath))
                                    Directory.CreateDirectory(DestinationPath);

                                FileUpload.MoveTo(_path);
                            }
                            else
                            {
                                _path = FileUpload.FullName;
                                _FileName = Regex.Replace(_FileName, @"\d{2,}.", ".");
                            }

                            allAttachedFiles.Add(_path); //list to send email
                            EmailAttachmentInsert emailAttachmentUI = new EmailAttachmentInsert();//Change from UI to Insert

                            if (savedAttachmentDetails != null)
                            {
                                var attachmentFileName = savedAttachmentDetails.Where(x => x.SavedFilePath.Contains(_FileName)).ToList();
                                if (attachmentFileName != null && attachmentFileName.Count() > 0)
                                {
                                    emailAttachmentUI.EmailOriginalFileName = attachmentFileName[0].OriginalAttachmentName.ToString();
                                }
                            }
                            else
                            {

                                emailAttachmentUI.EmailOriginalFileName = _FileName;
                                emailAttachmentUI.EmailID = EmailId;
                                emailAttachmentUI.CaseID = CaseId;
                                emailAttachmentUI.EmailFilePath = _path.Replace(System.Web.HttpContext.Current.Server.MapPath("~/Attachments"), "").TrimStart(new char[] {'/', '\\' });
                                emailAttachmentUI.Isactive = true;
                                emailAttachmentUI.Createdon = DateTime.Now;
                                emailAttachmentUI.Country = Country;
                                emailAttachmentUI.LastActedBy = Environment.UserName;
                                emailAttachmentUI.LastActedOn = DateTime.Now;

                                if (savedAttachmentDetails != null)
                                {
                                    var attachment = savedAttachmentDetails.Where(x => x.SavedFilePath.Contains(_FileName) && x.IsInline == true).ToList();
                                    if (attachment != null && attachment.Count() > 0)
                                    {
                                        emailAttachmentUI.IsInline = true;
                                    }

                                    var attachmentFileName = savedAttachmentDetails.Where(x => x.SavedFilePath.Contains(_FileName)).ToList();
                                    if (attachmentFileName != null && attachmentFileName.Count() > 0)
                                    {
                                        emailAttachmentUI.EmailOriginalFileName = attachmentFileName[0].OriginalAttachmentName.ToString();
                                    }

                                }

                                var emailAttachment = Task.Run(() => new EmailData().InsertIntoEmailAttachmentTable(emailAttachmentUI)).Result;
                                logger.Info("File attachment saved successfully");

                            }

                        }

                    }

                    catch (Exception ex)
                    {

                        logger.Info("Exception occured new case: saving email attachment to DB, Direction of the email: " + Direction);
                        logger.Info(ex.Message);
                        logger.Info(ex.StackTrace);
                        //Cleaning the attachments if it failed while saving one of the attachments.

                        foreach (string file in allAttachedFiles)
                        {
                            System.IO.File.Delete(file);
                        }

                        if (Direction == "Incoming")
                        {
                            logger.Debug("Before cleaning up email traces for email id: " + EmailId);
                            new EmailData().CleanupEmailTraces(EmailId);
                        }

                        throw ex;

                    }

                }

            }

            return allAttachedFiles;

        }
        public static void UpdateCaseIDInEmail(int CaseId, int EmailId)
        {

            //TODO: Update the case data with most recently inserted email
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                var caseDetail = db.CaseDetails.First(x => x.CaseID == CaseId);
                Email updateEmail = db.Emails.Where(x => x.EmailID == EmailId).FirstOrDefault(); //update email with caseID created above
                if (!updateEmail.EmailSubject.Contains("|CaseID:") && !updateEmail.EmailSubject.Contains("|ID:"))
                    updateEmail.EmailSubject = updateEmail.EmailSubject + " |ID:" + caseDetail.CaseIdIdentifier + "| ";
                else
                    updateEmail.EmailSubject = Regex.Replace(updateEmail.EmailSubject, @"\|ID:\s*.*2(?=\|)\|", " |ID:" + caseDetail.CaseIdIdentifier + "|");
                updateEmail.Country = caseDetail.Country;
                updateEmail.CaseID = CaseId;
                db.Entry(updateEmail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
        }
        public static int? GetCaseIdFromEmailSubject(string EmailSubject, string MailboxCountry, ref string refCaseIdIdentifier)
        {
            using (CQCMSDbContext db = new CQCMSDbContext())
            {
                Dictionary<string, string> ExtractionFieldsSubj = null;
                int? CaseId = null;
                string CaseIdIdentifier = "";
                ExtractionFieldsSubj = new Dictionary<string, string>();
                ExtractionFieldsSubj.Add(" |CaseID:", @"|CaseID :\s*\d+\|");// CaseID- PK, consider only when its inside a pipe example - |CaseID: 12345]
                ExtractionFieldsSubj.Add("|ID:", @"\|ID:\s*.*2(?=\|)\|"); //CaseIdIdentifier
                if (EmailSubject == null)
                {
                    EmailSubject = "";
                }
                ExtractionFieldsSubj = ExtractEmailFields(ExtractionFieldsSubj, EmailSubject);
                if (!string.IsNullOrWhiteSpace(ExtractionFieldsSubj["|CaseID:"]))
                {
                    int InputCaseld = int.Parse(ExtractionFieldsSubj["|CaseID:"].Replace("|", ""));
                    CaseDataVM CaseInfo = new CaseDataVM();
                    CaseInfo = Task.Run(() => new CaseAllData().GetCaseDataByCaseID(MailboxCountry, InputCaseld)).Result;
                    if (CaseInfo != null)
                    {
                        var MailboxList = Convert.ToString(MailboxCountry).Split(';').ToList();
                        if (MailboxList.Contains(CaseInfo.Country))
                            CaseId = Convert.ToInt32(CaseInfo.CaseID);
                    }
                    logger.Info("Case ID retrieved from the subject: " + CaseId);
                    Console.WriteLine("Case ID retrieved from the subject: " + CaseId);
                }
                if (!string.IsNullOrWhiteSpace(ExtractionFieldsSubj["|ID:"]))
                {
                    CaseIdIdentifier = ExtractionFieldsSubj["|ID:"].Replace("|", "").Replace("~", "#");
                    logger.Info("Case Identifier retrieved from the subject: " + CaseIdIdentifier);
                    Console.WriteLine("Case Identifier retrieved from the subject: " + CaseIdIdentifier);
                    if (CaseId == null || CaseId == 0)
                    {
                        var caseDetail = Task.Run(() => new CaseAllData().GetCaseByCaseldentifier(MailboxCountry, CaseIdIdentifier.Trim())).Result;
                        if (caseDetail != null)
                        {
                            CaseId = Convert.ToInt32(caseDetail.CaseID);
                            logger.Info("CaseID found from db by invoking GetCaseByCaseIdentifier: " + CaseId);
                        }
                    }
                }
                if (CaseId == 0) //will handle when CA case identifer exists in both CA and US, so it emails will attach properly, we can have same case identit
                {
                    CaseId = null;
                }
                refCaseIdIdentifier = CaseIdIdentifier;
                return CaseId;

            }

        }

        public static Dictionary<string, string> ExtractEmailFields(Dictionary<string, string> FieldsToExtract, String EmailBody)
        {
            var extractedValues = new Dictionary<string, string>();

            foreach (var fieldPair in FieldsToExtract)
            {
                string fieldValue = "";
                var regexResult = Regex.Match(EmailBody, fieldPair.Value);
                if (regexResult.Success)
                {
                    List<string> splitvalues = regexResult.Value.Split(':').ToList();
                    foreach (var s in splitvalues)
                    {
                        if (!fieldPair.Key.Contains(s))
                        {
                            fieldValue = s;
                            break;

                        }
                    }
                }
                extractedValues.Add(fieldPair.Key, fieldValue);
            }

            return extractedValues;
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/CleanupEmailTraces")]
        public static void CleanupEmailTraces(int Emailid)
        {

            logger.Debug("Staring to cleanup email traces for email id: " + Emailid);

            try
            {
                new EmailData().CleanupEmailTraces(Emailid);
            }
            catch (Exception ex)
            {
                logger.Debug("Exception occured in cleaning email traces: " + ex.Message);
                throw ex;
            }
        }

    }
}



