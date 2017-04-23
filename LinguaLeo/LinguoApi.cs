using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LinguaLeo
{
    public class LinguaLeoApi
    {
        private static CookieWebClient WebClient { get; set; }

        public Translate[] Translate { get; set; }
        public bool is_new { get; set; }

        //public static Translate GetTranslation(string word)
        //{
        //    using (var webclient = new CookieWebClient())
        //    {
        //        webclient.QueryString.Add("word", word);
        //        webclient.Encoding = Encoding.UTF8;
        //        var response = webclient.DownloadString(@"http://api.lingualeo.com/gettranslates");
        //        var json = JsonConvert.DeserializeObject<LinguaLeoApi>(response);
        //        return json.Translate.FirstOrDefault();
        //    }
        //}

        public static async Task<bool> AddWord(string word, string translation, string email, string password)
        {
            WebClient = new CookieWebClient();
            await Auth(email, password);
            WebClient.QueryString.Add("word", word.Trim());
            if(!string.IsNullOrWhiteSpace(translation)) WebClient.QueryString.Add("tword", translation);
            WebClient.Encoding = Encoding.UTF8;
            
            var response = await WebClient.DownloadStringTaskAsync(@"http://api.lingualeo.com/addword");
            var json = JsonConvert.DeserializeObject<LinguaLeoApi>(response);

            WebClient.Dispose();

            return json.is_new;
        }

        public static async Task Auth(string email, string password)
        {
            WebClient.QueryString.Add("email", email);
            WebClient.QueryString.Add("password", password);
            await WebClient.DownloadStringTaskAsync(@"http://api.lingualeo.com/api/login");
        }

    }

    public class Translate
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int Votes { get; set; }
    }

    public class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; private set; }

        /// <summary>
        /// This will instanciate an internal CookieContainer.
        /// </summary>
        public CookieWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Use this if you want to control the CookieContainer outside this class.
        /// </summary>
        public CookieWebClient(CookieContainer cookieContainer)
        {
            CookieContainer = cookieContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null) return base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }
    }
}
