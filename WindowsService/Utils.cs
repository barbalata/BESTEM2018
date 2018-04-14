using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace WindowsService
{
    class Utils
    {
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        public static ServiceState GetServiceState(String SERVICE_NAME)
        {
            ServiceController sc;
            try
            {
                sc = new ServiceController(SERVICE_NAME);
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("Invalid service name."); // Note that just because a name is valid does not mean the service exists.
                return ServiceState.SERVICE_STOPPED;
            }

            ServiceControllerStatus status;
            try
            {
                sc.Refresh(); // calling sc.Refresh() is unnecessary on the first use of `Status` but if you keep the ServiceController in-memory then be sure to call this if you're using it periodically.
                status = sc.Status;
            }
            catch (Win32Exception ex)
            {
                // A Win32Exception will be raised if the service-name does not exist or the running process has insufficient permissions to query service status.
                // See Win32 QueryServiceStatus()'s documentation.
                Debug.WriteLine("Error: " + ex.Message);
                return ServiceState.SERVICE_STOPPED;
            }
            switch (status)
            {
                case ServiceControllerStatus.StartPending:
                    return ServiceState.SERVICE_START_PENDING;
                case ServiceControllerStatus.Running:
                    return ServiceState.SERVICE_RUNNING;
                case ServiceControllerStatus.Paused:
                    return ServiceState.SERVICE_PAUSED;
                case ServiceControllerStatus.PausePending:
                    return ServiceState.SERVICE_PAUSE_PENDING;
                case ServiceControllerStatus.Stopped:
                    return ServiceState.SERVICE_STOPPED;
                case ServiceControllerStatus.StopPending:
                    return ServiceState.SERVICE_STOP_PENDING;
                default:
                    return ServiceState.SERVICE_CONTINUE_PENDING;
            }
        }

        #region Install Service
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

        public static void InstallService(string serviceName, Assembly assembly)
        {
            if (IsServiceInstalled(serviceName))
            {
                return;
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
        private static AssemblyInstaller GetInstaller(Assembly assembly)
        {
            AssemblyInstaller installer = new AssemblyInstaller(assembly, null);
            installer.UseNewContext = true;

            return installer;
        }
        #endregion

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
