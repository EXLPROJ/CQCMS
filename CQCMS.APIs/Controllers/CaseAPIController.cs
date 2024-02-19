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
using CQCMS.Providers;
using CQCMS.Providers.DataAccess;
using System.Security.Policy;
using Microsoft.Ajax.Utilities;
//using System.EnterpriseServices;
using System.Net.Mail;
//using System.Web.Services.Description;
using System.Xml.Linq;
using System.Runtime.Remoting.Lifetime;

namespace CQCMS.API.Controllers
{
    public class CaseAPIController : Controller
    {
        // GET: CaseAPI
        private static Logger logger = LogManager.GetLogger("EmailTransformation");


        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/SaveCaseDetails")]
        public dynamic SaveCaseDetails(CaseAndEmailUpdateDTO caseUpdateDTO)
        {
            CaseDetailVM existingCase = new CaseDetailVM();
            caseUpdateDTO.CaseChangeDTO = new CaseChangeDTO();
            existingCase = new CaseAllData().GetCaseByCaseID(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.CaseID);
            bool IsNewCaseCreated = false;
            bool? isexternalsubcat = false;

            if (caseUpdateDTO.UpdateCase.CaseID == 0)
            {
                IsNewCaseCreated = true;
                logger.Info("Creating New Case");
                Guid Token = Guid.NewGuid();
                CaseDetail newCase;
                using (CQCMSDbContext db = new CQCMSDbContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            newCase = new CaseDetail

                            {
                                MailboxID = caseUpdateDTO.UpdateCase.MailboxID,
                                IsAssigned = false,
                                CaseStatusID = ((caseUpdateDTO.UpdateCase.CaseStatusID != null && caseUpdateDTO.UpdateCase.CaseStatusID != 0) ? caseUpdateDTO.UpdateCase.CaseStatusID : (int)CaseStatus.NewCase),
                                Createdon = DateTime.Now,
                                CreatedBy = caseUpdateDTO.UpdateCase.CreatedBy,
                                IsCaseClosed = false,
                                UniqueldentifierGUID = Token.ToString(),
                                Priority = caseUpdateDTO.UpdateCase.Priority,  //this is mail priority (high or normal)
                                IsComplaint = false,
                                IsPhoneCall = false,
                                IsFlagged = false, //this is user defined flag
                                FirstEmailID = caseUpdateDTO.UpdateCase.FirstEmailID,
                                LastEmailID = caseUpdateDTO.UpdateCase.LastEmailID,
                                IsCaseEscalated = caseUpdateDTO.UpdateCase.IsCaseEscalated,
                                CaseAssignAttempts = 0,
                                Country = caseUpdateDTO.UpdateCase.Country,
                                NewEmailCount = (caseUpdateDTO.UpdateCase.NewEmailcount != null ? caseUpdateDTO.UpdateCase.NewEmailcount : 1),
                                LastUpdateSentOn = DateTime.Today.AddDays(-5),
                                AccountNumber = caseUpdateDTO.UpdateCase.AccountNumber,
                                NoOfQueries = 1,
                                CIN = caseUpdateDTO.UpdateCase.CIN,
                                ClientName = caseUpdateDTO.UpdateCase.ClientName,
                                KeepWithMe = caseUpdateDTO.UpdateCase.KeepWithMe,
                                CaseIdIdentifer = caseUpdateDTO.UpdateCase.CaseIdIdentifier,
                                MultiAccountNumber = caseUpdateDTO.UpdateCase.MultiAccountNumber,
                            };

                            db.CaseDetails.Add(newCase);
                            db.SaveChanges();
                            transaction.Commit();

                        }
                        catch (SqlException ex)

                        {
                            logger.Error("Exception in Creating Case" + ex);
                            Console.WriteLine("Exception occured: " + ex);
                            transaction.Rollback();
                            EmailAPIController.CleanupEmailTraces(caseUpdateDTO.UpdateCase.FirstEmailID.Value);
                            throw new NotImplementedException();
                        }
                    }
                    if (newCase != null)
                    {
                        if (string.IsNullOrEmpty(caseUpdateDTO.UpdateCase.CaseIdIdentifier))
                        {
                            caseUpdateDTO.UpdateCase.CaseIdIdentifier = CaseAllData.GenerateCaseldIdentifier(newCase.Country, newCase.CaseID);

                        }

                        caseUpdateDTO.CaseChangeDTO.IsNewCase = true;
                        EmailAPIController.UpdateCaseIDInEmail(newCase.CaseID, newCase.FirstEmailID.Value);
                        caseUpdateDTO.UpdateCase.CaseID = newCase.CaseID;
                        caseUpdateDTO.UpdateEmail.CaseID = newCase.CaseID;

                        caseUpdateDTO.UpdateCase.CaseIdIdentifier = CaseAllData.GenerateCaseIdIdentifier(newCase.Country, newCase.CaseID);

                        // HashTagAPTController.AttachHashTagsToCaseld(newCase.CaseID, newCase.MailboxID, newCase.AccountNumber, newCase.Country);
                        FindChangedValue(existingCase, caseUpdateDTO.UpdateCase, false, caseUpdateDTO.CurrentUserId, false, false, "");

                        if (caseUpdateDTO.UpdateCase.IsCaseEscalated == true)
                            new CaseAllData().InsertIntoCaseAuditTrailTable("Case is escalated.", newCase.CaseID, caseUpdateDTO.CurrentUserId, "Edit");
                    }
                }
            }
            if (caseUpdateDTO.UpdateCase.CaseID != 0)
            {

                if (existingCase == null)
                {

                    existingCase = new CaseAllData().GetCaseByCaseID(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.CaseID);

                }
                if (existingCase.Country.ToLower().Contains(";") && !string.IsNullOrEmpty(caseUpdateDTO.UpdateCase.SelectedCaseCountry))
                {
                    caseUpdateDTO.UpdateCase.CaseIdIdentifier = CaseAllData.GenerateCaseIdIdentifier(caseUpdateDTO.UpdateCase.SelectedCaseCountry, caseUpdateDTO.UpdateCase.CaseID);
                    caseUpdateDTO.UpdateCase.Country = caseUpdateDTO.UpdateCase.SelectedCaseCountry;
                }

                if (existingCase.CategoryID == 0 || existingCase.CategoryID == null)
                {
                    if (caseUpdateDTO.UpdateCase.CategoryID != 0 && caseUpdateDTO.UpdateCase.CategoryID != null)
                        caseUpdateDTO.CaseChangeDTO.IsCaseClassifiedforFirstTime = true;
                }
                if (caseUpdateDTO.UpdateCase.AccountNumber != null && caseUpdateDTO.UpdateCase.AccountNumber != "")
                {

                    var accountlength = ConfigData.GetConfigValue("AccountNumberLength", caseUpdateDTO.UpdateCase.Country);
                    if (!string.IsNullOrWhiteSpace(accountlength))
                    {

                        int accLength = Convert.ToInt32(accountlength);
                        caseUpdateDTO.UpdateCase.AccountNumber = caseUpdateDTO.UpdateCase.AccountNumber.PadLeft(accLength, '0');

                    }
                }
                if (existingCase.Comments != null && caseUpdateDTO.UpdateCase.Comments != null)
                {
                    if (!caseUpdateDTO.UpdateCase.Comments.Equals(existingCase.Comments))
                    {

                        caseUpdateDTO.CaseChangeDTO.IsNewCommentAddedToComplaints = true;

                    }
                }

                // only to be called while force classification
                if ((caseUpdateDTO.UpdateCase.CategoryID != null && existingCase.CategoryID != caseUpdateDTO.UpdateCase.CategoryID) ||
                        (caseUpdateDTO.UpdateCase.SubCategoryID != null && existingCase.SubCategoryID != caseUpdateDTO.UpdateCase.SubCategoryID))
                {

                    caseUpdateDTO.CaseChangeDTO.Reclassified = true;

                }
                //if (caseUpdateDTO.UpdateCase.CategoryID != null)
                //{

                //    string CategoryName = Task.Run(() => new CategoryData().GetCategorybyCategoryID(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.CategoryID)).Result.CategoryName;
                //    string gpiCatName = ConfigData.GetConfigValue("GPI_Categoryname");
                //    if (CategoryName == gpiCatName && (caseUpdateDTO.CaseChangeDTO.IsCaseClassifiedforFirstTime == true || caseUpdateDTO.CaseChangeDTO.Reclassified == true))
                //    {


                //        logger.Info("case is classified to HVP ,saving the last emailid and status to search pending" + caseUpdateDTO.UpdateCase.CaseID);
                //        new GPIData().UpdateGPILastEmailIdInCase(caseUpdateDTO.UpdateCase.CaseID);
                //    }
                //}
                //Added the below code to help the case route and trigger assigncase function
                bool IsMLRecommended = caseUpdateDTO.UpdateCase != null && existingCase.NOQ_1 != null && (existingCase.CaseStatusID == 1 || existingCase.CaseStatusID == 3);
                if ((caseUpdateDTO.UpdateCase.ClientName != existingCase.ClientName || caseUpdateDTO.UpdateCase.AccountNumber != existingCase.AccountNumber)
                && (existingCase.CategoryID != null || existingCase.SubCategoryID != null) && (existingCase.CaseStatusID == 1 || existingCase.CaseStatusID == 3))

                {

                    caseUpdateDTO.CaseChangeDTO.Reclassified = true;
                }

                //if (caseUpdateDTO.UpdateCase.NoOfQueries != 1 && existingCase.NoOfQueries != 1 && existingCase.NoOfQueries != caseUpdateDTO.UpdateCase.NoOfQueries)
                //{
                //    new AssignmentAPIController().UpdateCapacityandSLAForQueryCountChange(existingCase.CaseID, caseUpdateDTO.UpdateCase.NoOfQueries,
                //    existingCase.Country, caseUpdateDTO.CurrentUserId);
                //    caseUpdateDTO.CaseChangeDTO.IsNoOfQueriesChanged = true;
                //}
                //try
                //{
                //if (caseUpdateDTO.HashTagDTO != null && = null && (caseUpdateDTO.HashTagDTO.PrevTags != caseUpdateDTO.HashTagDT0.FinalTags))
                //{
                //    caseUpdateDTO.CaseChangeDTO.IsHashTagChanged = true;
                //    var result = new HashTagAPIController().UpdateFamilyAndTagOnCase(existingCase.CaseID, caseUpdateDTO.HashTagDTO.HashtagsAndSources,
                //    caseUpdateDTO.HashTagDTO.IsHashTagEnabled, caseUpdateDTO.HashTagDTO.UserId);

                //}
                //}
                //catch (Exception ex)

                //{

                //    logger.Info("exception in saving tags");
                //    logger.Info(ex.Message);
                //    logger.Info(ex.StackTrace);
                //    throw ex;


                //}
                if (IsNewCaseCreated == false)
                {


                    FindChangedValue(existingCase, caseUpdateDTO.UpdateCase, false, caseUpdateDTO.CurrentUserId, false, false, "");
                }


                CaseDetailUI caseDetailUI = new CaseDetailUI()
                {
                    CIN = caseUpdateDTO.UpdateCase.CIN,
                    ClientName = caseUpdateDTO.UpdateCase.ClientName,
                    IsFlagged = caseUpdateDTO.UpdateCase.IsFlagged,
                    AccountNumber = caseUpdateDTO.UpdateCase.AccountNumber,
                    IsComplaint = caseUpdateDTO.UpdateCase.IsComplaint,
                    IsPhoneCall = caseUpdateDTO.UpdateCase.IsPhoneCall,
                    FollowUpDate = caseUpdateDTO.UpdateCase.FollowUpDate,
                    BusinessSegment = caseUpdateDTO.UpdateCase.BusinessSegment != null ? caseUpdateDTO.UpdateCase.BusinessSegment : "",
                    BusinessLineCode = (!string.IsNullOrEmpty(caseUpdateDTO.UpdateCase.BusinessLineCode) && caseUpdateDTO.UpdateCase.BusinessLineCode != "--Choose Your Value--") ? caseUpdateDTO.UpdateCase.BusinessLineCode : existingCase.BusinessLineCode,
                    //IsFeeReversal = caseUpdateDTO.UpdateCase.IsFeeReversal ?? existingCase.IsFeeReversal,
                    FeeReversalReason = caseUpdateDTO.UpdateCase.FeeReversalReason ?? existingCase.FeeReversalReason,
                    //FeeReversalAmount = caseUpdateDTO.UpdateCase.FeeReversalamount ?? existingCase.FeeReversalamount,
                    CategoryID = caseUpdateDTO.UpdateCase.CategoryID ?? existingCase.CategoryID,
                    SubCategoryID = caseUpdateDTO.UpdateCase.SubCategoryID ?? existingCase.SubCategoryID,
                    PendingStatus = caseUpdateDTO.UpdateCase.PendingStatus ?? existingCase.PendingStatus,
                    Comments = caseUpdateDTO.UpdateCase.Comments,
                    ComplaintOn = caseUpdateDTO.UpdateCase.ComplaintOn,
                    LastActedOn = DateTime.Now,
                    LastActedBy = caseUpdateDTO.UpdateCase.LastActedBy,
                    CreatedOn = existingCase.Createdon,
                    CreatedBy = existingCase.CreatedBy,
                    MailboxID = existingCase.MailboxID,
                    //EchoCaseNumber = caseUpdateDTO.UpdateCase.EchoCaseNumber ?? existingCase.EchoCasellumber,
                    CurrentlyAssignedTo = caseUpdateDTO.UpdateCase.CurrentlyAssignedTo ?? existingCase.CurrentlyAssignedTo,
                    PreviouslyAssignedTo = caseUpdateDTO.UpdateCase.PreviouslyAssignedTo ?? existingCase.PreviouslyAssignedTo,
                    IsAssigned = existingCase.IsAssigned,
                    AssignedTime = existingCase.AssignedTime,
                    CaseStatusID = existingCase.CaseStatusID,
                    AdditionalClientInfo = caseUpdateDTO.UpdateCase.AdditionalClientInfo ?? existingCase.AdditionalClientInfo,
                    IsCaseClosed = existingCase.IsCaseClosed,
                    ClosedOn = existingCase.Closedon,
                    ClosedBy = existingCase.ClosedBy,
                    CaseAdditionalDetail = caseUpdateDTO.UpdateCase.CaseAdditionalDetail ?? existingCase.CaseAdditionalDetail,
                    LastEmailID = caseUpdateDTO.UpdateCase.LastEmailID ?? existingCase.LastEmailID,
                    FirstEmailID = caseUpdateDTO.UpdateCase.FirstEmailID ?? existingCase.FirstEmailID,
                    Priority = caseUpdateDTO.UpdateCase.Priority ?? existingCase.Priority,
                    SLADueDate = caseUpdateDTO.UpdateCase.SLADueDate ?? existingCase.SLADueDate,
                    TouchDueDate = caseUpdateDTO.UpdateCase.TouchDueDate ?? existingCase.TouchDueDate,
                    CaseReOpenedOn = caseUpdateDTO.UpdateCase.CaseReOpenedOn ?? existingCase.CaseReOpenedOn,
                    CaseAssignAttempts = caseUpdateDTO.UpdateCase.CaseAssignAttempts ?? existingCase.CaseAssignAttempts,
                    DoesPartialSubjectMatch = caseUpdateDTO.UpdateCase.DoesPartialSubjectMatch ?? existingCase.DoesPartialSubjectMatch,
                    //IsCaseComplaintIntegrated = caseUpdateDTO.UpdateCase.IsCaseComplaintIntegrated ?? existingCase.IsCaseComplaintIntegrated,
                    MatchedPartialCases = caseUpdateDTO.UpdateCase.MatchedPartialCases ?? existingCase.MatchedPartialCases,
                    Country = caseUpdateDTO.UpdateCase.Country ?? existingCase.Country,
                    KeepWithMe = caseUpdateDTO.UpdateCase.KeepWithMe,
                    //NoOFQueries = caseUpdateDTO.UpdateCase.NoOFQueries == 0 ?? 1 : caseUpdateDTO.UpdateCase.NoOfQueries,
                    //EscalationRootCauseID = caseUpdateDTO.UpdateCase.EscalationRootCauseID,
                    //EscalationOriginatorID = caseUpdateDTO.UpdateCase.EscalationOriginatorID,
                    //EscalationOriginatorName = caseUpdateDTO.UpdateCase.EscalationOriginatorName,
                    //ReClassificationTriggeredBy = (caseUpdateDTO.UpdateCase.ReClassificationTriggeredBy : caseUpdateDTO.UpdateCase.ReClassificationTriggeredBy),
                    //ForceReclassificationBy = (caseUpdateDTO.UpdateCase.ForceReclassificationBy == null ? existingCase.ForceReclassificationBy : caseUpdateDTO.UpdateCase.ForceReclassificationBy),
                    //IsCaseAcknowledged = (caseUpdateDTO.UpdateCase.IsCaseAcknowledged == null ? existingCase.IsCaseAcknowledged : caseUpdateDTO.UpdateCase.IsCaseAcknowledged),

                };
                try {
                    caseDetailUI.CaseID = existingCase.CaseID;
                    existingCase = Task.Run(() => new CaseAllData().UpdateCaseUIDataByCaseID(caseUpdateDTO.UpdateCase.Country, caseDetailUI)).Result;
                    EmailAPIController.UpdateCaseIDInEmail(existingCase.CaseID, caseDetailUI.LastEmailID.Value);
                }

                catch (Exception ex)
                {
                    logger.Info("exception found on step 2");
                    logger.Info(ex.Message);
                    logger.Info(ex.StackTrace);
                    throw ex;
                }
                //if (caseUpdateDTO.UpdateCase.IsComplaint == true)
                //{
                //    var complaintcase = new ComplaintsAPIController().UpdateCreateComplaintsCase(caseUpdateDTO.UpdatedClassifierModel, existingCase,| caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.CurrentUserId);
                //}
            }
            try
            {
                if (caseUpdateDTO.CustomData != null) {
                    var result = Task.Run(() => new CaseAllData().UpdateSubmittedAttributeByCaseId(existingCase.CaseID, caseUpdateDTO.CustomData)).Result;

                }


            }
            catch (Exception ex) {
                logger.Info("exception in saving attribute");
                logger.Info(ex.Message);
                logger.Info(ex.StackTrace);
                throw ex;
            }


            try
            {
                //if (caseUpdateDTO.GPIDataList != null) {
                //    logger.Info("Saving Gpi Details for CaseID:" + existingCase.CaseID);
                //    new GPIData().SaveGPIDetails(existingCase.CaseID, existingCase.Country, caseUpdateDTO.CurrentUserId, caseUpdateDTO.GPIDataList);
                //    logger.Info("Gpi Details saved for CaseID:" + existingCase.CaseID);
                //    new GPIData().UpdateGPIStatus(existingCase.CaseID, caseUpdateDTO.CurrentUserId, (int)HSBC.THOR.Bolt2.Entities.Models.GPIStatus.GPIManuallyUpdated);
                //}
            }

            catch (Exception ex)
            {
                logger.Info("exception in saving GPIDetails");
                logger.Info(ex.Message);
                logger.Info(ex.StackTrace);
                throw ex;

            }


            if (caseUpdateDTO.UpdateCase.CategoryID != null && !existingCase.IsCaseClosed && !caseUpdateDTO.IsSavedCalledAfterMICategorization)
            {
                string forwardingemailaddress = string.Empty;
                SubCategoryVM sub = Task.Run(() => new CategoryData().GetSubCategorybySubCategoryIDAsync(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.SubCategoryID)).Result;

                if (sub != null)
                {

                    forwardingemailaddress = sub.ExternalEmailID;
                    isexternalsubcat = sub.IsExternalClient;

                }

                else
                    isexternalsubcat = false;

                if (isexternalsubcat != null && isexternalsubcat == true)
                {
                    if (forwardingemailaddress == null || forwardingemailaddress == "")
                    {
                        throw new Exception("Forwarding email not specified for the external client");
                    }
                    else
                    {

                        EmailVM Email = Task.Run(() => new EmailData().GetEmailbyEmailIdAsync(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.LastEmailID)).Result;
                        //Get attachments for the email and set actual attachment path then pass to the send email
                        List<EmailAttachmentVM> allInlineAttachments = Task.Run(() => new EmailData().GetEmailAttachementByEmailId(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.LastEmailID.Value, false)).Result;
                        List<EmailAttachmentVM> allAttachments = Task.Run(() => new EmailData().GetEmailAttachementByEmailId(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.LastEmailID.Value, true)).Result;

                        List<string> inlineImagePaths = new List<string>();

                        List<string> fileAttachmentsPath = new List<string>();

                        inlineImagePaths.AddRange(allInlineAttachments.Where(x => x.IsInline == true).Select(x => (Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Attachments"), x.EmailFilePath))));
                        fileAttachmentsPath.AddRange(allAttachments.Select(x => (Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Attachments"), x.EmailFilePath))));

                        new CaseAllData().InsertIntoCaseAuditTrailTable("It pass through case, case will be closed and mail will be forwarded to " + forwardingemailaddress, caseUpdateDTO.UpdateCase.CaseID, caseUpdateDTO.CurrentUserId, "Create");

                        //new EmailsAPIController().SendEmail(caseUpdateDTO.UpdateCase.LastEmailID.Value, Email.EmailBody, caseUpdateDTO.UpdateCase.LastActedBy,
                        //                       caseUpdateDTO.UpdateCase.CaseIdIdentifier, forwardingemailaddress, "", "", caseUpdateDTO.UpdateCase.Country, "Send Close",
                        //null, caseUpdateDTO.UpdateCase.LastEmailID.Value, inlineImagePaths,
                        //fileAttachmentsPath, "", caseUpdateDTO.UpdateCase.CategoryID, "Send Close");
                    }
                }
            }

            if (isexternalsubcat == false)
            {
                try
                {

                    if (caseUpdateDTO.UpdateCase.SubCategoryID != null && isexternalsubcat == false)

                        AssignmentAPIController.SetSLAandTouch(caseUpdateDTO.UpdateCase.CaseID, caseUpdateDTO.UpdateCase.SubCategoryID.Value, caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.CurrentUserId, false);
                }
                catch (Exception ex)
                {
                    logger.Info("Exception in setslaandtouch");
                    logger.Info(ex.Message);
                    logger.Info(ex.StackTrace);
                    throw ex;
                }
                try {
                    caseUpdateDTO.CaseChangeDTO.CaseID = caseUpdateDTO.UpdateCase.CaseID;
                    caseUpdateDTO.CaseChangeDTO.Country = caseUpdateDTO.UpdateCase.Country;
                    caseUpdateDTO.AssignmentNeeded = AssignmentAPIController.VerifyCaseAssignmentNeed(caseUpdateDTO.CaseChangeDTO, caseUpdateDTO.CurrentUserId);
                    AssignmentAPIController assignmentAPIController = new AssignmentAPIController();
                    if (caseUpdateDTO.AssignmentNeeded)
                    {
                        logger.Info("Calling AssignCase() from SaveCaseDetails: " + caseUpdateDTO.UpdateCase.CaseID);
                        caseUpdateDTO.UpdateCase = assignmentAPIController.AssignCase(new CaseChangeDTO { CaseID = caseUpdateDTO.UpdateCase.CaseID, Country = caseUpdateDTO.UpdateCase.Country, CurrentUserId = caseUpdateDTO.UpdateCase.LastActedBy });



                    }
                }

                catch (Exception ex)
                {
                    logger.Info("Exception in assigning case: " + ex.Message);
                    logger.Info(ex.Message);
                    logger.Info(ex.StackTrace);
                    throw ex;
                }
            }

            caseUpdateDTO.UpdateCase = new CaseAllData().GetCaseByCaseID(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.CaseID);
            if (caseUpdateDTO.UpdateCase.CurrentlyAssignedTo != caseUpdateDTO.CurrentUserId && caseUpdateDTO.UpdateCase.CurrentlyAssignedTo != null)
            {


                var x = Task.Run(() => new CaseAllData().UpdateCaseNewEmailCount(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.CaseID)).Result;

            }
            caseUpdateDTO.UpdateCase.EmployeeName = (caseUpdateDTO.UpdateCase.CurrentlyAssignedTo == null ? null : new UserData().GetUserInfoByEmployeeID(caseUpdateDTO.UpdateCase.Country, caseUpdateDTO.UpdateCase.CurrentlyAssignedTo)?.EmployeeName);
            return caseUpdateDTO;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/UpdateFiledCasesToAssigned")]
        public void UpdateFiledCasesToAssigned()

        {
            try
            {
                new CaseAllData().UpdateFiledCasesToAssigned();
            }
            catch (Exception ex) { }
        }

        public void FindChangedValue(CaseDetailVM existingcase, CaseDetailVM UpdatedCasedetails, bool MLCategorization, string currentUserID, bool isCloneCase, bool isBabyCase, string CaseIdIdentifer)
        {
            try
            {

                string changedfields = "";
                var MailboxName = "";
                if (MLCategorization == false)
                {

                    if (existingcase == null)
                    {

                        if (UpdatedCasedetails.MailboxID != 0) {
                            var Mailbox = 0;//new MailboxData().GetMailboxbyID(UpdatedCasedetails.Country, UpdatedCasedetails.MailboxID);
                            if (Mailbox != null)
                                MailboxName = "test mailbox";//Mailbox.MailboxName;
                        }

                        if (UpdatedCasedetails.AccountNumber != null && UpdatedCasedetails.AccountNumber != "")
                        {
                            changedfields += "; AccountNumber : " + UpdatedCasedetails.AccountNumber;
                        }


                        if (UpdatedCasedetails.ClientName != null && UpdatedCasedetails.ClientName != "") {
                            changedfields += "; ClientName : " + UpdatedCasedetails.ClientName;
                        }
                        if (UpdatedCasedetails.CIN != null && UpdatedCasedetails.CIN != null)
                        {
                            changedfields += "; CIN : " + UpdatedCasedetails.CIN;
                        }
                        if (UpdatedCasedetails.BusinessLineCode != null && UpdatedCasedetails.BusinessLineCode != "")
                        {
                            changedfields += "; LOB : " + UpdatedCasedetails.BusinessLineCode;
                        }
                        if (UpdatedCasedetails.NoOfQueries != null && UpdatedCasedetails.NoOfQueries != 1 && UpdatedCasedetails.NoOfQueries != 0) {
                            changedfields += "; No OF Queries : " + UpdatedCasedetails.NoOfQueries;
                        }
                        //if (UpdatedCasedetails.IsComplaint != null && UpdatedCasedetails.IsComplaint = true)
                        //{
                        //    changedfields += "; Is Complaint : " + UpdatedCasedetails.IsComplaint;
                        //}
                        if (UpdatedCasedetails.IsFeeReversal != null && UpdatedCasedetails.IsFeeReversal == true) {
                            changedfields += "; Is FeeReversal : " + UpdatedCasedetails.IsFeeReversal;
                        }
                        if (UpdatedCasedetails.IsFlagged != null && UpdatedCasedetails.IsFlagged == true) {
                            changedfields += "; Is Flagged : " + UpdatedCasedetails.IsFlagged;
                        }
                        if (UpdatedCasedetails.CategoryID != null)
                        {

                            changedfields += "; Case Type : " + Task.Run(() => new CategoryData().GetCategorybyCategoryID(UpdatedCasedetails.Country, UpdatedCasedetails.CategoryID)).Result.CategoryName;
                        }

                        if (UpdatedCasedetails.SubCategoryID != null) {
                            changedfields += "; Sub Case Type : " + Task.Run(() => new CategoryData().GetSubCategorybySubCategoryID(UpdatedCasedetails.Country, UpdatedCasedetails.SubCategoryID)).Result.SubCategoryName;
                        }
                        if (UpdatedCasedetails.FollowUpDate != null)
                        {
                            changedfields += "; Follow-Up Date : " + UpdatedCasedetails.FollowUpDate;
                        }
                        if (UpdatedCasedetails.PendingStatus != null && UpdatedCasedetails.PendingStatus != "0")
                        {
                            changedfields += "; Pending Status : " + UpdatedCasedetails.PendingStatus;
                        }

                        if (changedfields != "")
                        {
                            changedfields = changedfields.Remove(0, 1);

                        }
                        if (isBabyCase == false && isCloneCase == false) {
                            var Comment = "A" + (UpdatedCasedetails.Country.Contains(";") ? "global" : UpdatedCasedetails.Country) + " Case has been created from mailbox(" + MailboxName + ") " + (changedfields != "" ? " with following details " + changedfields : "");
                            new CaseAllData().InsertIntoCaseAuditTrailTable(Comment, UpdatedCasedetails.CaseID, currentUserID, "Create");
                        }
                        else
                        {

                            new CaseAllData().InsertIntoCaseAuditTrailTable("A " + (isCloneCase == true ? "clone" : "baby") + " case has been created from " + CaseIdIdentifer + (changedfields != "" ? " with following details " + changedfields : ""), UpdatedCasedetails.CaseID, currentUserID, "Create");
                        }
                    }
                    else {
                        if (UpdatedCasedetails.AccountNumber != null && existingcase.AccountNumber != UpdatedCasedetails.AccountNumber)
                        {
                            changedfields += "; AccountNumber : " + UpdatedCasedetails.AccountNumber;
                        }
                        if (UpdatedCasedetails.ClientName != null && existingcase.ClientName != UpdatedCasedetails.ClientName)
                        {
                            changedfields += "; ClientName : " + UpdatedCasedetails.ClientName;
                        }
                        if (UpdatedCasedetails.CIN != null && existingcase.CIN != UpdatedCasedetails.CIN)
                        {
                            changedfields += "; CIN : " + UpdatedCasedetails.CIN;
                        }
                        if (UpdatedCasedetails.BusinessLineCode != null && existingcase.BusinessLineCode != UpdatedCasedetails.BusinessLineCode)
                        {
                            changedfields += "; LOB : " + UpdatedCasedetails.BusinessLineCode;
                        }
                        if (UpdatedCasedetails.NoOfQueries != null && existingcase.NoOfQueries != UpdatedCasedetails.NoOfQueries) {
                            {
                                changedfields += "; No OF Queries : " + UpdatedCasedetails.NoOfQueries;
                            }
                            if (existingcase.IsComplaint != UpdatedCasedetails.IsComplaint) { }
                            changedfields += "; Is Complaint : " + UpdatedCasedetails.IsComplaint;
                        }
                        if (existingcase.KeepWithMe != UpdatedCasedetails.KeepWithMe) {
                            changedfields += "; Keep With Me : " + UpdatedCasedetails.KeepWithMe;
                        }

                        if (existingcase.IsFeeReversal != UpdatedCasedetails.IsFeeReversal) {
                            changedfields += "; Is FeeReversal : " + UpdatedCasedetails.IsFeeReversal;
                        }
                        if (existingcase.IsFlagged != UpdatedCasedetails.IsFlagged) {
                            changedfields += "; Is Flagged : " + UpdatedCasedetails.IsFlagged;
                        }
                        if (UpdatedCasedetails.CategoryID != null && existingcase.CategoryID != UpdatedCasedetails.CategoryID)
                        {

                            changedfields += "; Case Type : " + Task.Run(() => new CategoryData().GetCategorybyCategoryID(UpdatedCasedetails.Country, UpdatedCasedetails.CategoryID)).Result.CategoryName;

                        }

                        if (UpdatedCasedetails.SubCategoryID != null && existingcase.SubCategoryID != UpdatedCasedetails.SubCategoryID)
                        {
                            changedfields += "; Sub-Case Type : " + Task.Run(() => new CategoryData().GetSubCategorybySubCategoryID(UpdatedCasedetails.Country, UpdatedCasedetails.SubCategoryID)).Result.SubCategoryName;
                        }

                        if (UpdatedCasedetails.FollowUpDate != null && existingcase.FollowUpDate != UpdatedCasedetails.FollowUpDate) {

                            changedfields += "; Follow-Up Date : " + UpdatedCasedetails.FollowUpDate;
                        }

                        if (UpdatedCasedetails.PendingStatus != "0" && existingcase.PendingStatus != UpdatedCasedetails.PendingStatus) {

                            changedfields += "; Pending Status : " + UpdatedCasedetails.PendingStatus;
                        }


                        if (changedfields != "")
                        {
                            changedfields = changedfields.Remove(0, 1);
                        }

                        if (changedfields != "") {
                            new CaseAllData().InsertIntoCaseAuditTrailTable("Case Saved with following details" + changedfields, UpdatedCasedetails.CaseID, currentUserID, "Edit");

                        }
                    }
                }
                else
                {

                    if (UpdatedCasedetails.CategoryID != null && existingcase.CategoryID != UpdatedCasedetails.CategoryID)
                    {
                        changedfields = " Case Type : " + Task.Run(() => new CategoryData().GetCategorybyCategoryID(UpdatedCasedetails.Country, UpdatedCasedetails.CategoryID)).Result.CategoryName;

                    }
                    if (UpdatedCasedetails.SubCategoryID != null && existingcase.SubCategoryID != UpdatedCasedetails.SubCategoryID)
                    {

                        changedfields += " & Sub Case Type : " + Task.Run(() => new CategoryData().GetSubCategorybySubCategoryID(UpdatedCasedetails.Country, UpdatedCasedetails.SubCategoryID)).Result.SubCategoryName;
                    }


                    if (UpdatedCasedetails.CategoryID != null && UpdatedCasedetails.SubCategoryID != null && changedfields != "")
                    {
                        new CaseAllData().InsertIntoCaseAuditTrailTable("Case is classified by ML model with " + changedfields, UpdatedCasedetails.CaseID, currentUserID, "Edit");

                    }
                }
            }
            catch (Exception ex) {
            }
        }



        [Http.HttpPost]
        [Http.Route("api/ReleaseCases")]
        [System.Web.Http.AllowAnonymous]

        public string ReleaseCases(ReleaseCaseDTO releaseCase)

        {
            CaseDetailVM caseData = new CaseDetailVM();
            caseData = new CaseAllData().GetCaseByCaseID(releaseCase.Country, releaseCase.CaseId);
            if (caseData != null)

            {
                try
                {
                    logger.Info("Releasing case to " + (releaseCase.ReleaseToUser == null ? " Pool" : releaseCase.ReleaseToUser) + " from: " + releaseCase.ReleaseFromUser + " for caseid: " + caseData.CaseID + " at time: " + DateTime.Now + ",By:" + releaseCase.CurrentUserId ?? "From BOT");
                    if (releaseCase.ReleaseFromUser != "Escalated")
                    {

                        //Task.Run(() => AssignmentAPIController.UpdateUserWieightedCapacity(caseData.SubCategoryID, releaseCase.Country, caseData.NoOfQueries, false, releaseCase.CurrentUserId));
                        new UserData().CapacityUpdateForAllUsers();
                    }

                    CaseRelease caseRelease = new CaseRelease();
                    caseRelease.CaseId = caseData.CaseID;
                    caseRelease.AssignedTime = caseData.AssignedTime;

                    if (releaseCase.ReleaseFromUser == "Escalated")

                    {
                        caseRelease.CaseEscalatedManager = releaseCase.ReleaseToUser;
                        caseRelease.PreviouslyAssignedTo = caseData.PreviouslyAssignedTo;
                        caseRelease.CurrentlyAssignedTo = caseData.CurrentlyAssignedTo;
                        caseRelease.CurrentlyAssignedTo = caseData.CurrentlyAssignedTo;
                        caseRelease.IsAssigned = caseData.IsAssigned;

                    }

                    else if (caseData.IsCaseClosed == true) //for re-opened cases

                    {
                        caseRelease.PreviouslyAssignedTo = releaseCase.ReleaseToUser;
                        caseRelease.CaseEscalatedManager = caseData.CaseEscalatedManager;

                    }

                    else if (releaseCase.ReleaseToUser != null)

                    {
                        caseRelease.PreviouslyAssignedTo = (caseData.CurrentlyAssignedTo != null ? caseData.CurrentlyAssignedTo : null);
                        caseRelease.CurrentlyAssignedTo = releaseCase.ReleaseToUser;
                        caseRelease.IsAssigned = true;
                        caseRelease.AssignedTime = DateTime.Now;
                        caseRelease.CaseEscalatedManager = caseData.CaseEscalatedManager;

                    }

                    else if (releaseCase.ReleaseToUser == null && caseData.IsCaseClosed != true && caseData.CurrentlyAssignedTo != null)
                    {
                        caseRelease.PreviouslyAssignedTo = caseData.CurrentlyAssignedTo;
                        caseRelease.CurrentlyAssignedTo = null;
                        caseRelease.IsAssigned = false;
                        caseRelease.caseAssignAttempts = 0;
                        caseRelease.AssignedTime = null;

                    }

                    caseRelease.LastActedby = releaseCase.CurrentUserId;

                    caseRelease.LastActedOn = DateTime.Now;

                    caseRelease.country = releaseCase.Country;

                    var newCase = Task.Run(() => new CaseAllData().UpdateCaseRelease(releaseCase.Country, caseRelease)).Result;

                    if (releaseCase.ReleaseFromUser != "Escalate" && releaseCase.ReleaseToUser != null && newCase.CurrentlyAssignedTo != null)//only adjust capacity if non esclation case release to reguler user.

                    {

                        //Task.Run(() => AssignmentAPIController.UpdateUsertei ghtedCapacity(caseData.SubCategoryID, releaseCase.Country,                        keseData.NoOfQueries, true, newCase.CurrentlyAssignedTo));
                        new UserData().CapacityUpdateForAllUsers();
                    }

                    var x = Task.Run(() => new CaseAllData().UpdateCaseNewEmailCount(releaseCase.Country, newCase.CaseID)).Result;
                    return "Sucess";

                }

                catch (Exception ex)

                {
                    logger.Info("error in Releasing Cases" + ex.Message);
                    return "Fail";

                }
            }
            return "Fail";

        }


    }
}

