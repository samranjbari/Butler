using System;
using System.ComponentModel;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Butler
{
    public partial class Form1 : Form
    {
        public BuildResults LastBuildResult { get; set; }

        public Form1()
        {
            InitializeComponent();

            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            this.notifyIcon1.ContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);
            this.notifyIcon1.ContextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(ContextMenuStrip_ItemClicked);

            timer1.Start();
        }

        private void AddSubStrip(ToolStripMenuItem strip, string text)
        {
            var itemsToRemove = new List<ToolStripItem>();
            foreach (ToolStripItem item in contextMenuStrip1.Items)
            {
                if (item.Text.Equals(strip.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (var item in itemsToRemove)
            {
                contextMenuStrip1.Items.Remove(item);
            }

            var sub = new ToolStripMenuItem(text);
            strip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] 
            {
                sub
            });
        }

        void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // get latest build
            AllJobs jobs = JsonHelpers.GetJsonDataFor<AllJobs>(@"http://localhost:8080/api/json");

            foreach (var job in jobs.Jobs)
            {
                var strip = new System.Windows.Forms.ToolStripMenuItem(job.Name);

                JenkinsJob jobOne = JsonHelpers.GetJsonDataFor<JenkinsJob>(string.Format(@"http://localhost:8080/job/{0}/api/json", job.Name));

                if (jobOne.LastBuild != null)
                {
                    var jobDetail = JsonHelpers.GetJsonDataFor<JenkinsJobDetails>(string.Format("{0}/api/json", jobOne.LastBuild.Url));
                    var causeJson = JsonConvert.SerializeObject(jobDetail.Actions[0]);
                    jobDetail.Causes = JsonConvert.DeserializeObject<CausesObject>(causeJson);

                    AddSubStrip(strip, string.Format("Last build was {0} by {1}", jobDetail.Result, jobDetail.Causes.Causes[0].UserName));
                    AddSubStrip(strip, "Build Now");

                    contextMenuStrip1.Items.Insert(0, strip);
                }
            }
        }

        void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var clickedItem = e.ClickedItem.Text.ToUpper();

            switch (clickedItem)
            {
                case "EXIT":
                    Application.Exit();
                    break;
                case "SETTINGS":
                    new SettingsForm().Show();
                    break;
                default:
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string message = string.Empty;
            try
            {
                timer1.Stop();

                // get latest build
                JenkinsJob job = JsonHelpers.GetJsonDataFor<JenkinsJob>(@"http://localhost:8080/job/CCWeb/api/json");

                JenkinsJobDetails jobDetails = JsonHelpers.GetJsonDataFor<JenkinsJobDetails>(string.Format("{0}/api/json", job.LastBuild.Url));

                var causeJson = JsonConvert.SerializeObject(jobDetails.Actions[0]);
                jobDetails.Causes = JsonConvert.DeserializeObject<CausesObject>(causeJson);

                if (jobDetails.Result.Equals("FAILURE", StringComparison.InvariantCultureIgnoreCase) && this.LastBuildResult != BuildResults.FAILURE)
                {
                    this.LastBuildResult = BuildResults.FAILURE;
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
                    this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Error")));

                    notifyIcon1.ShowBalloonTip(5000, "Build Failed", "Build Failed", ToolTipIcon.Error);
                }
                if (jobDetails.Result.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase) && this.LastBuildResult != BuildResults.SUCCESS)
                {
                    this.LastBuildResult = BuildResults.SUCCESS;
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
                    this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));

                    notifyIcon1.ShowBalloonTip(5000, "Build Is now back to normal", "Build Is back to normal", ToolTipIcon.Info);
                }

                timer1.Start();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                if (!string.IsNullOrEmpty(message))
                {
                    notifyIcon1.ShowBalloonTip(5000, "Invalid Jenkins Server or Job Name", message, ToolTipIcon.Error);
                }
                else
                {
                    timer1.Start();
                }
            }
        }
    }
}
