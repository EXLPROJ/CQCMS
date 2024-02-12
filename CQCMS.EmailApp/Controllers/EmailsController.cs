using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CQCMS.CommonHelpers;
using CQCMS.EmailApp;
using CQCMS.EmailApp.Data;
using CQCMS.Entities;
using CQCMS.Entities.DTOs;
using CQCMS.Entities.Models;
using CQCMS.Providers.DataAccess;
using NLog;

namespace CQCMS.EmailApp.Controllers
{
    public class EmailsController : Controller
    {
        private CQCMSEmailAppContext db = new CQCMSEmailAppContext();
        private static Logger logger = LogManager.GetLogger("EmailTransformation");
        // GET: Emails
        public ActionResult Index()
        {
            return View(db.Emails.ToList());
        }

        // GET: Emails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Email email = db.Emails.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        // GET: Emails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Emails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmailID,CaseID,EmailTypeID,MailboxID,ReceivedOn,SentOn,LastActedOn,LastActedBy,CreatedOn,CreatedBy,EmailSubject,EmailFrom,EmailTo,EmailCC,EmailBCC,EmailFolder,EmailSubFolder,EmailStatus,EmailDirection,Priority,AwaitingReview,ReviewedOn,ReviewedBy,ReviewerEdited,IsEmailComplaintIntegrated,EmailTrimmedSubject,Country,EmailHash")] Email email,FormCollection form)
        {
            if (ModelState.IsValid)
            {
                email.ReceivedOn = DateTime.Now;
                string emailbody = form["EmailBody"];
                //db.Emails.Add(email);
                //db.SaveChanges();
                int EmailID = email.EmailID;

                CaseIdEmailIdDTO SavedEmailDetails;

                List<SavedAttachment> attachments;
                try
                {
                    // if (mail.attachments.count() > 0)
                    attachments = mailEngine.SaveAttachments(mail, AttachmentTempFolder, tempAttachmentPath);
                    attachmentPath = attachments != null ? string.Join(";", attachments.Select(x => x.SavedFilePath).ToArray()) : "";
                    // Save email

                    ApplicationHelper ApplicationHelper = new ApplicationHelper();
                SavedEmailDetails = ApplicationHelper.SaveNewEmailAndCreateNewCase(CaseId, currmailbox.MailboxID, "US", CaseId == null ?
                    (int)CaseStatus.FirstEmail : (int)CaseStatus.NewEmail, email.EmailSubject, emailbody
                , mail.Folder, mail.Receivedon, mail.Senton, attachmentPath, email.EmailFrom, null, null, email.EmailTo, email.EmailCC,
                AttachmentTempFolder, false, false, email.Priority, "Incoming", false, AutoReplyInfo, attachments);

                    System.IO.Directory.Delete(Path.Combine(tempAttachmentPath, AttachmentTempFolder), true);

                }

                catch (ApplicationException ex)
                {
                    Logger.Error("Exception gcgured in saving email to database, " + ex.Message);
                    //logger.Info ("Saved email details have emailid: " + SavedEmailDetails.Emailid + " and gaseid " + SavedEmailDetails.CaselId);
                    System.IO.Directory.Delete(Path.Combine(tempAttachmentPath, AttachmentTempFolder), true);

                    logger.Error("Moving to the next email after deleting temporary attachments");                    
                }





                EmailAttachmentInsert emailAttachmentUI = new EmailAttachmentInsert();//Change from UI to Insert
                emailAttachmentUI.EmailID = newFile.EmailID;
                emailAttachmentUI.CaseID = newFile.CaseID;
                emailAttachmentUI.EmailFileName = newFile.EmailFileName;
                emailAttachmentUI.EmailFilePath = newFile.EmailFilepath;
                emailAttachmentUI.Isactive = newFile.IsActive;
                emailAttachmentUI.Createdon = newFile.Createdon;
                emailAttachmentUI.Country = userCountry;
                emailAttachmentUI.LastActedon = DateTime.Now;
                emailAttachmentUI.LastActedBy = Environment.UserName;
                await new EmailData().InsertIntoEmailAttachmentTable(emailAttachmentUI);
                logger.Info("File attachment saved successfully");

                return RedirectToAction("Index");
                }

            return View(email);
        }

        // GET: Emails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Email email = db.Emails.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        // POST: Emails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmailID,CaseID,EmailTypeID,MailboxID,ReceivedOn,SentOn,LastActedOn,LastActedBy,CreatedOn,CreatedBy,EmailSubject,EmailFrom,EmailTo,EmailCC,EmailBCC,EmailFolder,EmailSubFolder,EmailStatus,EmailDirection,EchoStatus,EchoLockedOn,EchoLockedBy,EchoAttempts,IsEchoLocked,Priority,AwaitingReview,ReviewedOn,ReviewedBy,ReviewerEdited,IsEmailComplaintIntegrated,EmailTrimmedSubject,Country,EmailHash,EchoAttemptsNum")] Email email)
        {
            if (ModelState.IsValid)
            {
                db.Entry(email).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(email);
        }

        // GET: Emails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Email email = db.Emails.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        // POST: Emails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Email email = db.Emails.Find(id);
            db.Emails.Remove(email);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
