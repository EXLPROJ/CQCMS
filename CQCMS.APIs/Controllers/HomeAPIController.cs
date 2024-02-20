using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CQCMS.APIs.Controllers
{
    public class HomeAPIController : ApiController
    {

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/GetTempFileNames/{tempAttachFolderGuid}")]
        public dynamic GetTempFileNames(string tempAttachFolderGuid)
        {

            List<string> tempFileNames = new List<string>();

            string _path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Attachments/TemporaryAttachments/" + tempAttachFolderGuid));
            DirectoryInfo d = new DirectoryInfo(_path);
            FileInfo[] tempAllFiles = d.GetFiles();
            List<List<string>> Filelistwithpath = new List<List<string>>();

            foreach (FileInfo file in tempAllFiles)
            {
                List<string> temp = new List<string>();
                temp.Add(file.Name);
                temp.Add("/Attachments/TemporaryAttachments/" + tempAttachFolderGuid + "/" + HttpUtility.UrlEncode(file.Name).Replace("+", "%20"));
                Filelistwithpath.Add(temp);
            }
            return Filelistwithpath;
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/AttachFilesTemporarily/{tempAttachFolderGuid?}")]
        public dynamic AttachFilesTemporarily(string tempAttachFolderGuid = "")
        {
            if (string.IsNullOrEmpty(tempAttachFolderGuid))
            {
                tempAttachFolderGuid = HttpContext.Current.Request.Form["EmailID"];
            }

            HttpFileCollection MultipleFiles = HttpContext.Current.Request.Files;
            string currEmailid = HttpContext.Current.Request.Form["EmailID"];

            List<string> allAttachedFiles = new List<string>();
            if (MultipleFiles != null)
            {

                string tempFolderName = "";
                Guid tempFolderName1 = new Guid();
                if (string.IsNullOrEmpty(tempAttachFolderGuid))
                {
                    tempFolderName1 = Guid.NewGuid();
                    tempFolderName = "" + tempFolderName1;
                }
                else
                {
                    tempFolderName = tempAttachFolderGuid;
                }
                for (int i = 0; i < MultipleFiles.Count; i++)
                {
                    try
                    {

                        HttpPostedFile FileUpload = MultipleFiles[i];
                        if (FileUpload.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileName(FileUpload.FileName);
                            string _path = null;
                            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Attachments/TemporaryAttachments/")))
                            {

                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Attachments/TemporaryAttachments/"));
                            }

                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Attachments/TemporaryAttachments/" + tempFolderName) + "/");
                            _path = Path.Combine(HttpContext.Current.Server.MapPath("~/Attachments/TemporaryAttachments/" + tempFolderName), _FileName);
                            if (System.IO.File.Exists(_path))
                            {
                                Random newFileExt = new Random();
                                int fileExt = newFileExt.Next(1, 9999);

                                _FileName = Path.GetFileNameWithoutExtension(FileUpload.FileName) + " (" + fileExt + ")" + Path.GetExtension(FileUpload.FileName);
                                _path = Path.Combine(HttpContext.Current.Server.MapPath("~/Attachments/TemporaryAttachments/" + tempFolderName), _FileName);

                                FileUpload.SaveAs(_path);
                            }
                            else
                            {
                                FileUpload.SaveAs(_path);
                            }
                            allAttachedFiles.Add(_path); //list to send email
                        }

                    }
                    catch (Exception ex)
                    {
                        return "File upload failed! !";
                    }
                }
                var result = new { tempfolder = tempFolderName.ToString(), curremailid = currEmailid };
                return result;
            }
            return null;

        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/RemoveTempFiles/{tempAttachFolderGuid}/{fileName}")]
        public void RemoveTempFiles(string tempAttachFolderGuid, string fileName)
        {

            fileName = fileName.Replace("2EDOT", ".");
            string _path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Attachments/TemporaryAttachments/" + tempAttachFolderGuid));
            DirectoryInfo d = new DirectoryInfo(_path);
            FileInfo[] tempAllFiles = d.GetFiles(fileName);
            foreach (FileInfo file in tempAllFiles)
            {
                file.Delete();
            }
        }
    }
}