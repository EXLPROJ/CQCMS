using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CQCMS.EmailApp;
using CQCMS.EmailApp.Data;

namespace CQCMS.EmailApp.Controllers
{
    public class EmailsController : Controller
    {
        private CQCMSEmailAppContext db = new CQCMSEmailAppContext();

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
        public ActionResult Create([Bind(Include = "EmailID,CaseID,EmailTypeID,MailboxID,ReceivedOn,SentOn,LastActedOn,LastActedBy,CreatedOn,CreatedBy,EmailSubject,EmailFrom,EmailTo,EmailCC,EmailBCC,EmailFolder,EmailSubFolder,EmailStatus,EmailDirection,EchoStatus,EchoLockedOn,EchoLockedBy,EchoAttempts,IsEchoLocked,Priority,AwaitingReview,ReviewedOn,ReviewedBy,ReviewerEdited,IsEmailComplaintIntegrated,EmailTrimmedSubject,Country,EmailHash,EchoAttemptsNum")] Email email,FormCollection form)
        {
            if (ModelState.IsValid)
            {
                email.ReceivedOn = DateTime.Now;
                string emailbody = form["EmailBody"];
                db.Emails.Add(email);
                db.SaveChanges();
                int EmailID = email.EmailID;
                

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
