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
        public static T GetJsonDataFor<T>(string url)
        {
            Configuration config = new Configuration();
            config = config.Load();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}/api/json", config.JenkinsServerUrl, url));
            string username = config.UserName;
            string password = config.Password;
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
