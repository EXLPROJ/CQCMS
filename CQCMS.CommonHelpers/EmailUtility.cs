using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Web;

namespace CQCMS.CommonHelpers
{
    public class EmailUtility
    {
        public static string ConvertHtmlBodyToTextBody(string HtmlBody)
        {
            string resultstring = "";
            try
            {

                Console.WriteLine("Started convert to html");

                var htmlDocument = new HtmlAgilityPack.HtmlDocument();

                //Modification done by Sybratg on 17-May-21 for reading encrypted emails

                if (HtmlBody.Contains("BEGIN VOLTAGE SECURE BLOCK") && HtmlBody.Contains("END VOLTAGE SECURE BLOCK"))
                {
                    int indexOfVoltageSecureStart = HtmlBody.IndexOf("BEGIN VOLTAGE SECURE BLOCK");
                    int indexOfVoltageSecureEnd = HtmlBody.IndexOf("END VOLTAGE SECURE BLOCK");
                    string strippedBodyText = HtmlBody.Remove((indexOfVoltageSecureStart - 38), ((indexOfVoltageSecureEnd - 35) - (indexOfVoltageSecureStart - 38)));
                    //MessageBody strippedBody = (MessageBody) strippedBodyText;
                    htmlDocument.LoadHtml(strippedBodyText);
                }
                else
                {
                    htmlDocument.LoadHtml(HtmlBody);
                }

                HtmlNode bodyNode = htmlDocument.DocumentNode.SelectSingleNode("//body");
                if (bodyNode == null)
                {
                    Console.WriteLine("body was null..going to decode");
                    return HttpUtility.HtmlDecode(htmlDocument.DocumentNode.InnerText.Replace("&nbsp;", " "));
                }
                Console.WriteLine("body was not null..going to decode");


                var task = System.Threading.Tasks.Task.Run(() => HttpUtility.HtmlDecode(bodyNode.InnerText.Replace("spkspi", " ")));
                if (task.Wait(TimeSpan.FromSeconds(25)))
                {
                    resultstring = task.Result;
                    Console.WriteLine("HtmlDecode Worked.");

                }
                else
                {
                    Console.WriteLine("HtmlDecode Timed out.");
                    throw new Exception("HtmlDecode Timed out.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HtmlDecode did not work: " + ex.Message);
            }
            return resultstring;
        }
    }
}