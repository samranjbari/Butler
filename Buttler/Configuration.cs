// -----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Butler
{
    public class Configuration
    {
        public string JenkinsServerUrl { get; set; }
        public List<string> JobNames { get; set; }
        public int TimerInterval { get; set; }

        public Configuration()
        {
            TimerInterval = 10000;
            JobNames = new List<string>();
        }

        public void Save()
        {
            string config = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText("./config.json", config);
        }

        public Configuration Load()
        {
            var configFile = File.ReadAllText("./config.json");
            var config = JsonConvert.DeserializeObject<Configuration>(configFile);
            return config;
        }
    }
}
