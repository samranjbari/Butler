using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Newtonsoft.Json;

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
            try
            {
                // get latest build
                AllJobs jobs = JsonHelpers.GetJsonDataFor<AllJobs>(string.Empty);

                foreach (var job in jobs.Jobs)
                {
                    var strip = new System.Windows.Forms.ToolStripMenuItem(job.Name);

                    JenkinsJob jobOne = JsonHelpers.GetJsonDataFor<JenkinsJob>(string.Format(@"/job/{0}", job.Name));

                    if (jobOne.LastBuild != null)
                    {
                        var jobDetail = JsonHelpers.GetJsonDataFor<JenkinsJobDetails>(string.Format("/job/{0}/{1}", jobOne.Name, jobOne.LastBuild.Number));
                        var causeJson = JsonConvert.SerializeObject(jobDetail.Actions[0]);
                        jobDetail.Causes = JsonConvert.DeserializeObject<CausesObject>(causeJson);

                        AddSubStrip(strip, string.Format("Last build was {0} by {1}", jobDetail.Result, jobDetail.Causes.Causes[0].UserName));
                        AddSubStrip(strip, "Build Now");

                        contextMenuStrip1.Items.Insert(0, strip);
                    }
                }
            }
            catch (Exception ex) { }
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
                    timer1.Start();
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
                JenkinsJob job = JsonHelpers.GetJsonDataFor<JenkinsJob>(@"/job/CCWeb/api/json");

                JenkinsJobDetails jobDetails = JsonHelpers.GetJsonDataFor<JenkinsJobDetails>(string.Format("/job/{0}/{1}", job.Name, job.LastBuild.Number));

                var causeJson = JsonConvert.SerializeObject(jobDetails.Actions[0]);
                jobDetails.Causes = JsonConvert.DeserializeObject<CausesObject>(causeJson);

                if (jobDetails == null || jobDetails.Result == null)
                {
                    return;
                }

                if (jobDetails.Result.Equals("FAILURE", StringComparison.InvariantCultureIgnoreCase) && this.LastBuildResult != BuildResults.FAILURE)
                {
                    this.LastBuildResult = BuildResults.FAILURE;
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
                    this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Error")));

                    notifyIcon1.ShowBalloonTip(10000, "Build Failed", jobDetails.Causes.Causes[0].UserName + " broke the build.", ToolTipIcon.Error);
                    this.notifyIcon1.Text = "Butler says Last Build Failed by " + jobDetails.Causes.Causes[0].UserName;
                }
                if (jobDetails.Result.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase) && this.LastBuildResult != BuildResults.SUCCESS)
                {
                    this.LastBuildResult = BuildResults.SUCCESS;
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
                    this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));

                    notifyIcon1.ShowBalloonTip(10000, "Build Is now back to normal", "Build Is back to normal", ToolTipIcon.Info);
                    this.notifyIcon1.Text = "Butler says All builds are good!";
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
