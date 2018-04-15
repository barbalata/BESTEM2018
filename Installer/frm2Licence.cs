using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Installer
{
    public partial class frm2Licence : Form
    {
        public frm2Licence()
        {
            InitializeComponent();
            this.btnNext.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel Intel(R) Network Connections installation?", "Intel(R) Network Connections Install Wizard", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            frm3Options options = new frm3Options();
            this.Hide();
            options.ShowDialog();
            this.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frm1Welcome welcome = new frm1Welcome();
            this.Hide();
            welcome.ShowDialog();
            this.Close();
        }

        private void radBtnNotAccept_CheckedChanged(object sender, EventArgs e)
        {
            this.btnNext.Enabled = false;
        }

        private void radBtnAccept_CheckedChanged(object sender, EventArgs e)
        {
            this.btnNext.Enabled = true;
        }

        private void frm2Licence_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
