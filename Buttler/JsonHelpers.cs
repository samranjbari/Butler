// -----------------------------------------------------------------------
// <copyright file="JsonHelpers.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.IO;
using Newtonsoft.Json;
namespace Butler
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class JsonHelpers
    {
        public Configuration Config { get; set; }
        public JsonHelpers(Configuration config)
        {
            this.Config = config;
        }

        //public static string GetJsonDataFor(string url)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    return new StreamReader(response.GetResponseStream()).ReadToEnd();
        //}

        public static T GetJsonDataFor<T>(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
