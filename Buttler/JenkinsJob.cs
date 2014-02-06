// -----------------------------------------------------------------------
// <copyright file="JenkinsJob.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Butler
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class JenkinsJob
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public bool Buildable { get; set; }
        public List<BuildInfo> Builds { get; set; }
        public string Color { get; set; }
        public BuildInfo LastBuild { get; set; }
        public List<HealthInfo> HealthReport { get; set; }
    }

    public class BuildInfo
    {
        public int Number { get; set; }
        public string Url { get; set; }
    }

    public class HealthInfo
    {
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int Score { get; set; }
        public string FullIcon
        {
            get
            {
                return string.Format("http://localhost:8080/static/acc794b1/images/24x24/{0}", IconUrl);
            }
        }
    }
}
