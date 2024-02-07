using CQCMS.Entities;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Text;
using CQCMS.Entities.DTOs;
//using CQCMS.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Principal;
using CQCMS.Entities.Models;

namespace CQCMS.CommonHelpers

{

    public class ApplicationHelper

    {

        //private static APIUtility<MailboxIdDT0> apiMailboxIdUtility = null;
        private static APIUtility<CaseIdEmailIdDTO> apiEmailAndCaseInsertDTOUtility = null;
        //private static APIUtility<MailboxIdDT0> apiEmailSearchUtility = null;
        //private static APIUtility<CaseReOpenDetail> apiCaseReopenUtility = null;
        //private static APIUtility<CaseDetailVM> apiCaseDetailVM = null;
        private static APIUtility<CaseAndEmailUpdateDTO> apiCaseAndEmailUpdateDTO = null;
        //private static APIUtility<Email> apiEmailReturns = null;
        //private static APIUtility<string> apiStringReturns = null;
        //private static APIUtility<bool> apiBoolReturns = null;

        public ApplicationHelper() : this("Default")
        {
        }

        public ApplicationHelper(string InvokerName)
        {
            //apiMailboxIdUtility = new APIUtility<MailboxIdDTO>(InvokerName);
            apiEmailAndCaseInsertDTOUtility = new APIUtility<CaseIdEmailIdDTO>(InvokerName);
            //apiEapiEmailSearchUtility = new APIUtility<MailboxIdDTO>(InvokerName);
            //apiCaseReopenUtility = new APIUtility<CaseReOpenDetai1>(InvokerName);
            //apiCaseDetailVM = new APIUtility<CaseDetailVM>(InvokerName);
            apiCaseAndEmailUpdateDTO = new APIUtility<CaseAndEmailUpdateDTO>(InvokerName);
            //apiEmailReturns = new APIUtility<Email>(InvokerName);
            //apiStringReturns = new APIUtility<string>(InvokerName);
            //apiBoolReturns = new APIUtility<bool>(InvokerName);

        }
        //public MailboxIdDTO GetMailboxPriorityFromReceipients(string ToRecipients, string CCRecipients, int MailboxId, string MailboxCountry, string EmailFolder)
        //{

        //    MailboxPriorityDTO mailboxPriorityDTO = new MailboxPriorityDTO
        //    {
        //        ToRecipients = ToRecipients,
        //        CCRecipients = CCRecipients,
        //        MailboxId = MailboxId,
        //        MailboxCountry = MailboxCountry,
        //        EmailFolder = EmailFolder
        //    };
        //    string url = ConfigurationManager.AppSettings["APIUr1"] + "api/GetMailboxPriorityFromReceipients";
        //    var result = apiMailboxIdUtility.GetResponse(url, HttpMethod.Post, JsonConvert.SerializeObject(mailboxPriorityDT0), true).GetAwaiter().GetResult();

        //    return result.FirstOrDefault();

        //}

        public CaseIdEmailIdDTO SaveNewEmailAndCreateNewCase(int? CaseID, int MailBoxId, string MailboxCountry, int CaseStatus, string EmailSubject, string EmailHtmlBody, string EmailFolder,
            DateTime? ReceivedOn, DateTime? SentOn, string AttachmentPath, string EmailFrom, int? CategoryId, int? SubcategoryId, string ToReceipients = "", string CcReceipients = "",
            string AttachmentTempFolder = null, bool SaveAsDraft = true, bool SendAndClose = false, string Priority = "Normal", string Direction = "Incoming", bool IsCaseEscalated = false,
            string AutoReplyInfo = null, List<SavedAttachment> savedattachmentDetails = null)

        {
            CaseAndEmailUpdateDTO caseAndEmailUpateDTO = new CaseAndEmailUpdateDTO();
            EmailVM updatetmail =
            new EmailVM
            {
                CaseID = CaseID,
                MailboxID = MailBoxId,
                Country = MailboxCountry,
                EmailStatus = CaseStatus,
                EmailSubject = EmailSubject,
                EmailBody = EmailHtmlBody,
                EmailFolder = EmailFolder,
                EmailFrom = EmailFrom,
                ReceivedOn = ReceivedOn,
                SentOn = SentOn,
                AttachmentPath = AttachmentPath,
                EmailTo = ToReceipients,
                EmailCC = CcReceipients,
                AttachmentTempFolder = AttachmentTempFolder,
                SaveAsDraft = SaveAsDraft,
                SendAndClose = SendAndClose,
                Priority = Priority,
                Direction = Direction,
                IsCaseEscalated = IsCaseEscalated,
                AutoReplyInfo = AutoReplyInfo,
                savedAttachmentDetails = savedattachmentDetails
            };

            caseAndEmailUpateDTO.UpdateEmail = updatetmail;
            string url = ConfigurationManager.AppSettings["APIUr1"] + "api/SaveNewEmailAndCreateCase";
            var result = apiEmailAndCaseInsertDTOUtility.GetResponseFromSaveNewEmailAndCreateCase(url, HttpMethod.Post, JsonConvert.SerializeObject(caseAndEmailUpateDTO)).GetAwaiter().GetResult();

            return result;
        }
    }
}