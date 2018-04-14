using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Text;
using System.ComponentModel;
using static WindowsService.Utils;
using System.Reflection;

namespace WindowsService
{
    
    public partial class Service1 : ServiceBase
    {
        private int eventId = 1;
        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "Intel(R) Network Connections";

            eventLog1 = new System.Diagnostics.EventLog();
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource("MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";

        }

        protected override void OnStart(string[] args)
        {
            #region Start service settings
            eventLog1.WriteEntry("In OnStart");

            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            #endregion

            #region My code
            using (FileStream fs = File.Create("C:\\Test.txt"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file." + DateTime.Now);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
            #endregion
        }

        protected override void OnStop()
        {
            base.OnStop();
            eventLog1.WriteEntry("In onStop.");
            Console.WriteLine("Stop");
            //Thread.Sleep(2000);
            //ServiceController service = new ServiceController("Intel(R) Network Connections");
            //service.Start();
        }


        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);

            String SERVICE_NAME = "";

            if (!IsServiceInstalled(SERVICE_NAME))
            {
                String filePath = AssemblyDirectory;
                Assembly assembly = Assembly.LoadFrom(filePath);
                InstallService(SERVICE_NAME, assembly);
            }

            if(GetServiceState(SERVICE_NAME) != ServiceState.SERVICE_RUNNING)
            {
                ServiceController service = new ServiceController(SERVICE_NAME);
                service.Start();
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
