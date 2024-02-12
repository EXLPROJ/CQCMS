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
using CQCMS.Entities.Models;
using CQCMS.Providers.DataAccess;
using CQCMS.EmailApp.Models;
using System.Security.Policy;

namespace CQCMS.API.Controllers
{
    public class EmailAPIController : Controller
    {
        private static Logger logger = LogManager.GetLogger("EmailTransformation");
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

                //var currentUser = new UserData().GetUserInfoByEmployeeID(updateDTO.UpdateEmail.Country, updateDTO.CurrentUserId);
                //var mailboxaddress = new MailboxData().GetMailboxbyID(updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.MailboxID).MailboxName;
                //if (currentUser != null && currentUser.EmployeeName != "" && mailboxaddress != null && mailboxaddress != "")

                //{
                //    updateDTO.UpdateEmail.EmailFrom = currentUser.EmployeeName + " <" + mailboxaddress + ">";
                //}
                try
                {

                    email = SaveEmail(updateDTO.UpdateEmail);

                    //if (updateDTO.UpdateCase != null && updateDTO.UpdateCase.CaseID == 0)
                    //{
                    //    if (string.IsNullOrEmpty(updateDTO.UpdateCase.AccountNumber))
                    //    {
                    //        var coroClientDetail = CoRoHelper.GetClientDetailFromCoRoByEmail(updateDTO.UpdateEmail.EmailFrom, updateDTO.UpdateEmail.Country);

                    //        if (coroClientDetail != null)

                    //        {
                    //            updateDTO.UpdateCase.AccountNumber = coroClientDetail.AccountNumbers.Split(',').Length > 1 ? null : coroClientDetail.AccountNumbers;
                    //            updateDTO.UpdateCase.CIN = coroClientDetail.CIN ?? null;
                    //            updateDTO.UpdateCase.ClientName = coroClientDetail.ClientName ?? null;
                    //            updateDTO.UpdateCase.MultiAccountNumber = coroClientDetail.AccountNumbers ?? null;
                    //            updateDTO.UpdateCase.Country = coroClientDetail.Country ?? null;
                    //        }

                    //    }

                    //    updateDTO.UpdateCase.Country = updateDTO.UpdateCase.Country ?? updateDTO.UpdateEmail.Country;
                    //    updateDTO.UpdateCase.FirstEmailID = email.EmailID;
                    //    updateDTO.UpdateCase.LastEmailID = email.EmailID;
                    //    updateDTO.UpdateCase.MailboxID = updateDTO.UpdateEmail.MailboxID;
                    //    updateDTO.UpdateCase.Priority = updateDTO.UpdateEmail.Priority;
                    //    updateDTO.UpdateCase.CurrentlyAssignedTo = updateDTO.CurrentUserId;
                    //    updateDTO.UpdateCase.CaseStatusID = (int)CaseStatus.CaseAssigned;
                    //    updateDTO.UpdateCase.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;
                    //    updateDTO.UpdateCase.CreatedBy = updateDTO.UpdateEmail.CurrentUserId;
                    //    updateDTO.UpdateCase.NewEmailcount = 0;

                    //    updateDTO.UpdateCase.KeepWithMe = true;

                    //    //updateDTO = new CaseAPIController().SaveCaseDetails(updateDTO);
                    //}
                    //else
                    //{

                    //    var CasedetailsUpdate = db.CaseDetails.First(x => x.CaseID == (int)updateDTO.UpdateCase.CaseID);
                    //    CasedetailsUpdate.LastEmailID = email.EmailID;
                    //    CasedetailsUpdate.LastViewedEmailID = email.EmailID;
                    //    CasedetailsUpdate.CaseStatusID = (CasedetailsUpdate.CaseStatusID == (int)CaseStatus.NewCase ? (int)CaseStatus.CaseAssigned : CasedetailsUpdate.CaseStatusID);// changing from new case to assigned
                    //    CasedetailsUpdate.NewEmailCount = ((CasedetailsUpdate.CaseIdIdentifer.Contains('#') == true || CasedetailsUpdate.CurrentlyAssignedTo != updateDTO.CurrentUserId) && updateDTO.UpdateEmail.EmailDirection.ToLower().Contains("close") == false ? (CasedetailsUpdate.NewEmailCount + 1) : 0);
                    //    CasedetailsUpdate.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;
                    //    CasedetailsUpdate.LastActedOn = DateTime.Now;

                    //    if (CasedetailsUpdate.IsCaseAcknowledged == null && updateDTO.UpdateCase.IsCaseAcknowledged == true)
                    //    {
                    //        CasedetailsUpdate.IsCaseAcknowledged = updateDTO.UpdateCase.IsCaseAcknowledged;
                    //        CasedetailsUpdate.CaseAckDateTime = DateTime.Now;
                    //        CasedetailsUpdate.AckMailId = email.EmailID;
                    //        CasedetailsUpdate.AckSentBy = email.CreatedBy;


                    //        db.Entry(CasedetailsUpdate).State = EntityState.Modified;
                    //        db.SaveChanges();

                    //        try
                    //        {
                    //            if (updateDTO.CustomData != null)
                    //            {
                    //                var result = Task.Run(() => new CaseAllData().UpdateSubmittedAttributeByCaseId(updateDTO.UpdateCase.CaseID, updateDTO.CustomData)).Result;
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            logger.Info("exception in saving attribute");
                    //            logger.Info(ex.Message);
                    //            logger.Info(ex.StackTrace);
                    //            throw ex;
                    //        }
                    //    }

                    //    Tuple<List<string>, List<string>, string> Attachments = SaveAttachmentsToFolderAndDb(updateDTO.UpdateEmail.AttachmentTempFolder, updateDTO.UpdateEmail.ReceivedOn.Value, updateDTO.UpdateEmail.CaseID.Value, updateDTO.UpdateEmail.AttachmentPath,
                    //    email.EmailID, email.EmailBody, updateDTO.UpdateEmail.Country, null, "Outgoing");

                    //    var OutgoingEmailBody = Attachments.Item3;
                    //    List<string> inlineImagePaths = Attachments.Item1;
                    //    List<string> fileAttachmentsPath = Attachments.Item2;

                    //    if (updateDTO.UpdateEmail.SaveAsDraft == false && !String.IsNullOrWhiteSpace(updateDTO.UpdateEmail.EmailTo))
                    //    {

                    //        SendEmail(email.EmailID, OutgoingEmailBody, updateDTO.UpdateEmail.CurrentUserId,
                    //        updateDTO.UpdateCase.CaseIdIdentifer, updateDTO.UpdateEmail.EmailTo, updateDTO.UpdateEmail.EmailCC, updateDTO.UpdateEmail.EmailBCC,
                    //        updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.Direction,
                    //        updateDTO.UpdateEmail.ListExistingFilesToRemoveForward, updateDTO.UpdateEmail.orginalEmailid, inlineImagePaths,
                    //        fileAttachmentsPath, updateDTO.UpdateEmail.UserName, updateDTO.UpdateCase.CategoryID,
                    //        updateDTO.UpdateEmail.EmailDirection, updateDTO.UpdateEmail.EmailClassificationLookup);
                    //    }

                    //    else

                    //    //TODO: Do something with email in this scenario

                    //    if (updateDTO.UpdateCase.CurrentlyAssignedTo == null)
                    //        updateDTO.UpdateCase = new CaseAllData().GetCaseByCaseID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.CaseID);
                    //    if (string.IsNullOrEmpty(Convert.ToString(updateDTO.UpdateCase.CurrentlyAssignedTo)))
                    //    {
                    //        updateDTO.UpdateCase.EmployeeName = null;
                    //    }
                    //    else
                    //    {
                    //        //case is assign to person by manually bulk assign who does not have access,code is breaking
                    //        //\With object reference error just avoid code breaking.
                    //        var employeeInfo = new UserData().GetUserInfoByEmployeeID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.CurrentlyAssignedTo);
                    //        if (employeeInfo != null)
                    //            updateDTO.UpdateCase.EmployeeName = employeeInfo.EmployeeName;
                    //        else
                    //            updateDTO.UpdateCase.EmployeeName = null;

                    //    }

                    //    if (updateDTO.UpdateCase.ParentCaseID != null && updateDTO.UpdateCase.ParentCaseID != 0 && updateDTO.UpdateCase.CaseIdIdentifier.Contains('#') == true)
                    //    {
                    //        CaseDetailVM ParentCase = new CaseAllData().GetCaseByCaseID(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.ParentCaseID);
                    //        return new CaseIdEmailIdDTO
                    //        {
                    //            EmailId = email.EmailID,
                    //            CaseId = updateDTO.UpdateEmail.CaseID,
                    //            CurrentlyAssignedTo = updateDTO.UpdateCase.CurrentlyAssignedTo,
                    //            EmployeeName = updateDTO.UpdateCase.EmployeeName,
                    //            CaseStatusID = updateDTO.UpdateCase.CaseStatusID,


                    //            CaseIdIdentifier = updateDTO.UpdateCase.CaseIdIdentifier,
                    //            ParentCaseAssignedTo = ParentCase.CurrentlyAssignedTo,
                    //            ParentCaseLastEmailId = (int)ParentCase.LastEmailID
                    //        };
                    //    }
                    //}
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
            //    if (email != null && email.EmailID != 0)
            //    {
            //        updateDTO.UpdateEmail.EmailID = email.EmailID;

                //        if (updateDTO.UpdateCase == null || updateDTO.UpdateCase.CaseID == 0)
                //            updateDTO.UpdateCase = new CaseDetailVM();

                //        if (updateDTO.UpdateEmail.CaseID == null)
                //        {
                //            updateDTO.UpdateEmail.CaseID = GetCaseIdFromEmailSubject(updateDTO.UpdateEmail.EmailSubject, updateDTO.UpdateEmail.Country, ref CaseIDIdentifier);
                //            updateDTO.UpdateCase.CaseIdIdentifier = CaseIDIdentifier;

                //            if (updateDTO.UpdateEmail.CaseID == null)
                //            {
                //                if (updateDTO.UpdateCase != null && updateDTO.UpdateCase.LastEmailID != null)
                //                {
                //                    updateDTO.UpdateEmail.CaseID = Task.Run(() => new CaseAllData().GetCaseByLastEmailId(updateDTO.UpdateCase.Country, updateDTO.UpdateCase.LastEmailID)).Result?.CaseID;
                //                }
                //                if (updateDTO.UpdateEmail.CaseID == null)
                //                {

                //                    logger.Info("Partial match logic started");
                //                    var resultPartialMatch = EmailData.FindPartiallyMatchingCases(updateDTO.UpdateEmail.EmailSubject,
                //                                            updateDTO.UpdateEmail.SentOn, updateDTO.UpdateEmail.ReceivedOn, email.TextBody, updateDTO.UpdateEmail.EmailTo,
                //                                            updateDTO.UpdateEmail.EmailCC, updateDTO.UpdateEmail.EmailFrom, updateDTO.UpdateEmail.MailboxID,
                //                                            updateDTO.UpdateEmail.Country);

                //                    if (resultPartialMatch != null && resultPartialMatch.Count == 1 && resultPartialMatch[0].CaseID != null)
                //                    {
                //                        //attach to existing case
                //                        updateDTO.UpdateEmail.CaseID = (int)resultPartialMatch[0].CaseID;
                //                        logger.Info("Match Found:Caseid:" + updateDTO.UpdateEmail.CaseID);
                //                    }
                //                    else
                //                    {
                //                        if (resultPartialMatch != null && resultPartialMatch.Count == 0)
                //                            logger.Debug("No matching cases found going to create case");

                //                        else if (resultPartialMatch != null && resultPartialMatch.Count > 1)
                //                        {
                //                            foreach (PartialCaseMatch pm in resultPartialMatch)
                //                            {
                //                                if (!PartiallyMatchedToCases.Contains(pm.CaseID.ToString()))
                //                                {

                //                                    PartiallyMatchedToCases = PartiallyMatchedToCases + pm.CaseID.ToString() + ";";
                //                                }
                //                            }
                //                            logger.Info("Partial subject match exception - Multiple matches found. Requires manual attachment");
                //                        }

                //                    }

                //                    if (updateDTO.UpdateEmail.CaseID == null)
                //                    {
                //                        if (updateDTO.UpdateCase == null || string.IsNullOrEmpty(updateDTO.UpdateCase.AccountNumber))
                //                        {
                //                            var coroClientDetail = CoRoHelper.GetClientDetailFromCoRoByEmail(updateDTO.UpdateEmail.EmailFrom, updateDTO.UpdateEmail.Country);

                //                            if (coroClientDetail != null && coroClientDetail.AccountNumbers != null)
                //                            {
                //                                updateDTO.UpdateCase.AccountNumber = coroClientDetail.AccountNumbers.Split(',').Length > 1 ? null : coroClientDetail.AccountNumbers;
                //                                updateDTO.UpdateCase.CIN = coroClientDetail.CIN ?? null;
                //                                updateDTO.UpdateCase.ClientName = coroClientDetail.ClientName ?? null;
                //                                updateDTO.UpdateCase.MultiAccountNumber = coroClientDetail.AccountNumbers ?? null;
                //                                //updateDTO.UpdateCase.Country = coroClientDetail.Country >? null;

                //                            }
                //                        }

                //                        updateDTO.UpdateCase.Country = updateDTO.UpdateCase.Country ?? updateDTO.UpdateEmail.Country;
                //                        updateDTO.UpdateCase.FirstEmailID = email.EmailID;
                //                        updateDTO.UpdateCase.LastEmailID = email.EmailID;
                //                        updateDTO.UpdateCase.MailboxID = updateDTO.UpdateEmail.MailboxID;
                //                        updateDTO.UpdateCase.Priority = updateDTO.UpdateEmail.Priority;
                //                        // updateDTO.UpdateCase.CurrentlyAssignedTo = updateDTO.CurrentUserId;
                //                        updateDTO.UpdateCase.IsCaseEscalated = updateDTO.UpdateEmail.IsCaseEscalated;
                //                        updateDTO.UpdateCase.CreatedBy = updateDTO.UpdateEmail.CurrentUserId;
                //                        updateDTO.UpdateCase.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;

                //                    }

                //                }
                //            }
                //        }
                //        if (updateDTO.UpdateEmail.CaseID != null)
                //        {

                //            CaseDetail CasedetailsUpdate;
                //            bool isEscalated = false;
                //            //var mailboxId = updateDTO.UpdateEmail.MailboxID;
                //            //var Mailboxdetails = db.Mailboxes.First(x => x.MailboxID == (int)mailboxId);
                //            //var Country = Mailboxdetails.Country;
                //            using (CQCMSDbContext db = new CQCMSDbContext())
                //            {
                //                using (var transaction = db.Database.BeginTransaction())
                //                {
                //                    try
                //                    {
                //                        CasedetailsUpdate = db.CaseDetail.First(x => x.CaseID == (int)updateDTO.UpdateEmail.CaseID);
                //                        CasedetailsUpdate.NewEmailCount = CasedetailsUpdate.NewEmailCount + 1;
                //                        if (!CasedetailsUpdate.IsCaseEscalated)
                //                        {
                //                            CasedetailsUpdate.IsCaseEscalated = updateDTO.UpdateEmail.IsCaseEscalated;
                //                            if (updateDTO.UpdateEmail.IsCaseEscalated)
                //                                isEscalated = true;
                //                        }

                //                        db.Entry(CasedetailsUpdate).State = EntityState.Modified;
                //                        db.SaveChanges();
                //                        transaction.Commit();
                //                        new CaseAllData().InsertIntoCaseAuditTrailTable("An email has been received."
                //                            + (isEscalated == true ? "Case is escalated." : ""), (int)updateDTO.UpdateEmail.CaseID, updateDTO.UpdateEmail.CurrentUserId, "email");
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        transaction.Rollback(); throw ex;
                //                    }
                //                }
                //            }

                //            updateDTO.UpdateCase = updateDTO.UpdateCase = new CaseAllData().GetCaseByCaseID(CasedetailsUpdate.Country, updateDTO.UpdateEmail.CaseID);
                //            updateDTO.UpdateCase.CaseID = (int)updateDTO.UpdateEmail.CaseID;
                //            updateDTO.UpdateCase.LastEmailID = updateDTO.UpdateEmail.EmailID;
                //            updateDTO.UpdateCase.LastActedOn = DateTime.Now;
                //            updateDTO.UpdateCase.LastActedBy = updateDTO.UpdateEmail.CurrentUserId;

                //        }
                //        updateDTO = new CaseAPIController().SaveCaseDetails(updateDTO);
                //        new CaseAPIController().UpdateFiledCasesToAssigned();
                //        Tuple<List<string>, List<string>, string> Attachments = SaveAttachmentsToFolderAndDb(updateDTO.UpdateEmail.AttachmentTempFolder,
                //        updateDTO.UpdateEmail.ReceivedOn.Value, updateDTO.UpdateCase.CaseID, updateDTO.UpdateEmail.AttachmentPath, email.EmailID,
                //        email.EmailBody, updateDTO.UpdateEmail.Country, updateDTO.UpdateEmail.savedAttachmentDetails, "Incoming");

                //    }

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
                //CleanupEmailTraces(email.EmailID);
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
                                UpdateEmail.TextBody = EmailUtility.ConvertHtmlBodyToTextBody(UpdateEmail.EmailBody);
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
                            Emailsubject = UpdateEmail.EmailSubject,
                            TextBody = UpdateEmail.TextBody,
                            LastActedon = DateTime.Now,
                            Createdon = DateTime.Now,
                            CreatedBy = UpdateEmail.CurrentUserId,
                            LastActedBy = UpdateEmail.CurrentUserId,
                            EmailDirection = UpdateEmail.Direction,
                            Priority = UpdateEmail.Priority,
                            Country = UpdateEmail.Country,
                            EmailTrimmedSubject = new EmailData().CleanEmailSubject(UpdateEmail.EmailSubject.Replace("'", "''")),
                            EmailHash = mailHash
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
                            Console.WriteLine(Environment.NewLine + "Email with subject:" + email.Emailsubject + " saved" + Environment.NewLine);
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
    }
}

