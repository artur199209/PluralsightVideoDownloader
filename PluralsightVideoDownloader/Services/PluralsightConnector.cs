using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PluralsightVideoDownloader.Models;
using Serilog;

namespace PluralsightVideoDownloader.Services
{
    public class PluralsightConnector
    {
        public static HttpClient PluralsightClient;

        public PluralsightConnector()
        {
            PluralsightClient = new HttpClient();
        }

        public List<Module> GetAllMoviesDataFromCourse(string courseName)
        {
            List<Module> modulesWithClips;
            var url = $"https://app.pluralsight.com/learner/content/courses/{courseName}";
            var client = new HttpClient();
            using (var response = client.GetAsync(url).Result)
            {
                using (HttpContent content = response.Content)
                {
                    string result = content.ReadAsStringAsync().Result;
                    var deserializeObject = (JObject)JsonConvert.DeserializeObject(result);
                    var deserializeModules = deserializeObject["modules"];

                    modulesWithClips = JsonConvert.DeserializeObject<List<Module>>(deserializeModules.ToString());
                }
            }

            return modulesWithClips;
        }

        public async Task<string> GetLinkToMovie(string title)
        {
            try
            {
                Log.Logger.Information("Start extracting movie link...");
                string movieLink;

                var paramJson = "{\"clipId\":\"{title}\",\"mediaType\":\"mp4\",\"quality\":\"1280x720\",\"online\":true,\"boundedContext\":\"course\",\"versionId\":\"\"}";
                var paramJsonWithTitle = paramJson.Replace("{title}", title);
                var stringContent = new StringContent(paramJsonWithTitle, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Cookie", "Version%7C4.4.0; profileScores=0|0|0|0|0|0|0|2; mbox=PC#bf8eb4d0cf454d6f92565b88e28c7935.37_0#1649662505|session#f08743ba720247f3b4b66432ff7d3c5d#1586418783; __cfduid=d1bdfcfcdac45cc7093786432641a718f1586154014; _psga=GA1.2.971832946.1586154022; _fbp=fb.1.1586154024958.2121665614; IR_PI=b05db15e-77ce-11ea-8f5f-42010a24660a%7C1586437472033; ei_client_id=5e8ebfa588a38a0010952ddd; _mkto_trk=id:306-DUP-745&token:_mch-pluralsight.com-1586154026662-89198; ps_optout=0; _sdsat_Target Subscription Info=; dyn_previousPage=https://app.pluralsight.com/player; ajs_user_id=%22b12c181b-7066-4bd7-8c8f-3db8056b2b40%22; ajs_group_id=null; ajs_anonymous_id=%2283e2b631-af7f-45ba-9ee3-ae1f063e4793%22; _sdsat_AJS User Cookie=%22b12c181b-7066-4bd7-8c8f-3db8056b2b40%22; NPS_a97f541b_last_seen=1586169787650; PsJwt-production=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJoYW5kbGUiOiJiMTJjMTgxYi03MDY2LTRiZDctOGM4Zi0zZGI4MDU2YjJiNDAiLCJpYXQiOjE1ODYxNjk3ODIsImV4cCI6MTU4Njc3NDU4Mn0.sItt5Y0KFyISmceMhFmti4YIIJmc-Y_oGkJLIKANBWQ; www-status-production=1; _sdsat_v03 - Global - User ID=b12c181b-7066-4bd7-8c8f-3db8056b2b40; _sdsat_v77 - SKUs & Slices=IND-M-PLUS; muxData=mux_viewer_id=86bdb913-3976-4653-8667-99bcce83f6bb&msn=0.519908654234268&sid=0a634095-660f-4735-b771-7aa0563054c0&sst=1586416926466&sex=1586418905688; _ga=GA1.2.891146297.1586170603; prism-cookienotify=1586170844995; __qca=P0-1875348513-1586170987515; _psga_gid=GA1.2.1571929724.1586348392; _gid=GA1.2.130228263.1586350915; mp_a439cb00b58dae694855aa14226ddf12_mixpanel=%7B%22distinct_id%22%3A%20%221715ac6ac96a-008834a712b629-4c302f7e-e1000-1715ac6ac9716c%22%2C%22%24device_id%22%3A%20%221715ac6ac96a-008834a712b629-4c302f7e-e1000-1715ac6ac9716c%22%2C%22mp_lib%22%3A%20%22Segment%3A%20web%22%2C%22%24initial_referrer%22%3A%20%22%24direct%22%2C%22%24initial_referring_domain%22%3A%20%22%24direct%22%7D; __RequestVerificationToken_L2lk0=yBFpIu6aORuqVh0GVaiTucCTMGebil--ZrZO76NlQvhBDBgyX2A89jD1yydUuKtiKOeDdZ65ZS2D6OXybPlU0DupUsI1; __cfruid=c919917386f234f16601d2afa577c12610d9d8fe-1586410337; check=true; QSI_HistorySession=https%3A%2F%2Fapp.pluralsight.com%2Fid%2F~1586362067650; AMCVS_70D658CC558978FF7F000101%40AdobeOrg=1; s_cc=true; s_sq=%5B%5BB%5D%5D; at_check=true; IR_gbd=pluralsight.com; IR_7490=1586413475393%7C0%7C1586413475393%7C%7C; hd_jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE1ODY5NzA3MDAsImlhdCI6MTU4NjM2NTkwMCwiYXVkIjoiaHR0cHM6Ly9oZWxwLnBsdXJhbHNpZ2h0LmNvbSIsInBlcm1pc3Npb25fZ3JvdXBzIjpbImdyb3VwOnlocW83eTB6ajEiXX0.ut5qtfVNmjdT3SyQ7uR9klMuj7DrJAVw_FBqZksSLVI; __cf_bm=7df683a21a7f1060d619c7e5492d89680282ba33-1586416921-1800-AVe1q1zpWX06RFlzQvMJJr0MjLOhcHT6jZdiCIwiFsBHYpK0jcyEwbAxk4MRKcb1ep7S9kpJYhpGJNfRIn6SGkA=; mboxEdgeCluster=37");
                    var response = await client.PostAsync("https://app.pluralsight.com/video/clips/v3/viewclip", stringContent);

                    var result = response.Content.ReadAsStringAsync().Result;
                    var jsonObject = (JObject)JsonConvert.DeserializeObject(result);
                    movieLink = jsonObject["urls"][3]["url"].ToString();
                    Log.Logger.Information($"Movie link: {movieLink}");
                }
                VideoDownloader videoDownloader = new VideoDownloader();
                videoDownloader.DownloadVideo(movieLink);
                return movieLink;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.ToString());
                Log.Logger.Error(e.StackTrace);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}