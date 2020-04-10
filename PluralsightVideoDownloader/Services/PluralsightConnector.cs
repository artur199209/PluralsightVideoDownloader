using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PluralsightVideoDownloader.Helpers;
using PluralsightVideoDownloader.Models;
using Serilog;

namespace PluralsightVideoDownloader.Services
{
    public class PluralsightConnector
    {
        public static HttpClient PluralsightClient;
        private readonly PluralsightSettings _pluralsightSettings;

        public PluralsightConnector(PluralsightSettings pluralsightSettings)
        {
            PluralsightClient = new HttpClient();
            _pluralsightSettings = pluralsightSettings;
        }

        public List<Module> GetAllMoviesDataFromCourse(string courseUrl)
        {
            List<Module> modulesWithClips;

            try
            {
                Log.Logger.Information("Get course name...");
                var courseName = PathHelper.GetCourseName(courseUrl);
                Log.Logger.Information($"Course name: {courseName}");
                var url = $"{_pluralsightSettings.LearnerPath}{courseName}";

                Log.Logger.Information("Get course data...");

                using (var response = PluralsightClient.GetAsync(url).Result)
                {
                    using (var content = response.Content)
                    {
                        var result = content.ReadAsStringAsync().Result;
                        var deserializeObject = (JObject)JsonConvert.DeserializeObject(result);
                        var deserializeModules = deserializeObject["modules"];

                        Log.Logger.Information("Successfully got modules info...");
                        modulesWithClips = JsonConvert.DeserializeObject<List<Module>>(deserializeModules.ToString());
                    }
                }

                foreach (var module in modulesWithClips)
                {
                    module.CourseName = courseName;

                    for (var i = 0; i < module.Clips.Count; i++)
                    {
                        var result = GetVideoAndTranscriptLinksToMovie(module.Clips[i].ClipId).Result;
                        module.Clips[i].MovieLink = result.Item1;
                        module.Clips[i].Title = $"{i+1}. {module.Clips[i].Title}";
                        module.Clips[i].TranscriptLink = result.Item2;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.ToString());
                Log.Logger.Error(e.StackTrace);
                Console.WriteLine(e);
                throw;
            }

            return modulesWithClips;
        }

        private async Task<Tuple<string, string>> GetVideoAndTranscriptLinksToMovie(string clipId)
        {
            try
            {
                Log.Logger.Information("Start extracting movie link...");
                Tuple<string, string> linksResult;
 
                var paramJson = "{\"clipId\":\"{clipId}\",\"mediaType\":\"mp4\",\"quality\":\"1280x720\",\"online\":true,\"boundedContext\":\"course\",\"versionId\":\"\"}";
                var paramJsonWithTitle = paramJson.Replace("{clipId}", clipId);
                var stringContent = new StringContent(paramJsonWithTitle, Encoding.UTF8, "application/json");
                var transcriptApi = _pluralsightSettings.TranscriptPath;
               
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Cookie",
                        "ps_trk=1; fv=1; AMCV_70D658CC558978FF7F000101%40AdobeOrg=1585540135%7CMCIDTS%7C18363%7CMCMID%7C08428151936998023158689638586185885249%7CMCAID%7CNONE%7CMCOPTOUT-1586506499s%7CNONE%7CvVersion%7C4.4.0; profileScores=0|0|0|0|0|0|0|0; at_check=true; __cfduid=d09d441edba65f7eeed166adc886c94c61586499262; __cfruid=b31c0ef184f334ce6fe9e7000fd3169ba9686048-1586499262; AMCVS_70D658CC558978FF7F000101%40AdobeOrg=1; s_cc=true; IR_gbd=pluralsight.com; IR_7490=1586499266071%7Cc-9961%7C1586499266071%7C%7C; s_sq=%5B%5BB%5D%5D; IR_PI=1586499266071.tuyad38z91r%7C1586585666071; __RequestVerificationToken_L2lk0=hvdsn63di4XKXAlbj1GCfOb3Rjqb6YnVndxtJ6n5enYq7yD9oBsyJzl9hQrsYlkxmImed1wIHmOYeW0exU8eTuLAW_U1; _sdsat_Target Subscription Info=; check=true; dyn_previousPage=https://app.pluralsight.com/library/courses/aspnet-core-fundamentals/table-of-contents; ajs_user_id=%2207e7b442-653d-4c1a-b4e3-5f586663531c%22; ajs_group_id=null; ajs_anonymous_id=%22de532821-575f-4130-8efa-778d736532b7%22; QSI_HistorySession=https%3A%2F%2Fapp.pluralsight.com%2Fid%3F~1586499269147; NPS_a97f541b_last_seen=1586499273978; PsJwt-production=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJoYW5kbGUiOiIwN2U3YjQ0Mi02NTNkLTRjMWEtYjRlMy01ZjU4NjY2MzUzMWMiLCJpYXQiOjE1ODY0OTkyNzMsImV4cCI6MTU4NzEwNDA3M30.eAz2PiZbCOnmu4N973MJ2VHdc_JWa8M3XH60BuiKGBY; www-status-production=1; _sdsat_AJS User Cookie=%2207e7b442-653d-4c1a-b4e3-5f586663531c%22; _sdsat_v03 - Global - User ID=07e7b442-653d-4c1a-b4e3-5f586663531c; _sdsat_v77 - SKUs & Slices=IND-M-PLUS; experimentCookie=profile-no-interest-selected-in-app-notification|2,personalized-newest-courses|control,learning_recommendations_toc_a/b|control; __cf_bm=2a64dc6032d9b5135abf9f51b6052aa20f15aae4-1586501827-1800-ASTgcTImmPToRAXUPZI+k3EhcpoohZ+g1HUoMYKW6Pi+KtuEQB9zCanhLkTfF7l+5CfBv5u9JmrRZd/meAAUveM=; mbox=session#5b9ab466499e4b71876a347ec3fdbb13#1586503691");
                    var response = await client.PostAsync(_pluralsightSettings.ViewClipPath, stringContent);

                    var result = response.Content.ReadAsStringAsync().Result;
                    var jsonObject = (JObject)JsonConvert.DeserializeObject(result);
                    Log.Logger.Information(jsonObject.ToString());
                    var movieLink = jsonObject["urls"]?[0]?["url"]?.ToString();
                    var version = jsonObject["version"].ToString();
                    var transcriptUrl = transcriptApi.Replace("{clipId}", clipId).Replace("{version}", version);
                    Log.Logger.Information($"Movie link: {movieLink}");
                    linksResult = new Tuple<string, string>(movieLink, transcriptUrl);
                    await Task.Delay(2500);
                }

                return linksResult;
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