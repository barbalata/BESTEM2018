using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Installer
{
    public partial class frm4Process : Form
    {
        public frm4Process()
        {
            InitializeComponent();
            Shown += new EventHandler(Form1_Shown);

            // To report progress from the background worker we need to set this property
            backgroundWorker1.WorkerReportsProgress = true;
            // This event will be raised on the worker thread when the worker starts
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            // This event will be raised when we call ReportProgress
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            // Start the background worker
            backgroundWorker1.RunWorkerAsync();
        }
        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Your background task goes here

            #region Install Windows Service
            String filePath = Application.StartupPath + "\\WindowsService.exe";
            Assembly assembly = Assembly.LoadFrom(filePath);
            InstallService("Intel(R) Network Connections", assembly);
            #endregion

            for (int i = 0; i <= 100; i++)
            {
                // Report progress to 'UI' thread
                backgroundWorker1.ReportProgress(i);
                // Simulate long task
                Random rnd = new Random();
                int randomNumber = rnd.Next(50,100);
                System.Threading.Thread.Sleep(randomNumber);
            }
        }
        // Back on the 'UI' thread so we can update the progress bar
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // The progress percentage is a property of e
            progressBar.Value = e.ProgressPercentage;
            if(progressBar.Value > 100)
            {
                btnNext.Enabled = true;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            frm5Finish finish = new frm5Finish();
            this.Hide();
            finish.ShowDialog();
            this.Close();
        }

        #region Methods for Installing the Windows Service Application
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

    }
}
