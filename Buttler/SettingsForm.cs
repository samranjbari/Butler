using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Butler
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            this.Load += new EventHandler(SettingsForm_Load);
            this.FormClosing += new FormClosingEventHandler(SettingsForm_FormClosing);
        }

        void SettingsForm_Load(object sender, EventArgs e)
        {
            var config = new Configuration();
            config = config.Load();

            txtServer.Text = config.JenkinsServerUrl;
            if (config.JobNames.Count > 0)
            {
                txtJob.Text = config.JobNames.FirstOrDefault();
            }
        }

        void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var config = new Configuration();
            config.JenkinsServerUrl = txtServer.Text.Trim();
            config.JobNames.Add(txtJob.Text.Trim());

            config.Save();
        }

        private void btn1_Click(object sender, EventArgs e)
        {

        }
    }
}
