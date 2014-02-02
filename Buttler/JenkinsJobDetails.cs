// -----------------------------------------------------------------------
// <copyright file="JenkinsJobDetails.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Butler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class JenkinsJobDetails
    {
        public List<object> Actions { get; set; }
        public bool Building { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int EstimatedDuration { get; set; }
        public string FullDisplayName { get; set; }
        public string Id { get; set; }
        public int Number { get; set; }
        public string Result { get; set; }
        public string Timestamp { get; set; }
        public string Url { get; set; }
        public CausesObject Causes { get; set; }
    }

    public class CausesObject
    {
        public List<Causes> Causes { get; set; }
    }

    public class Causes
    {
        public string ShortDescription { get; set; }
        public object UserId { get; set; }
        public string UserName { get; set; }
    }
}