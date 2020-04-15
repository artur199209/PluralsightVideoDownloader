using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PluralsightVideoDownloader.Helpers;
using PluralsightVideoDownloader.Models;
using Serilog;

namespace PluralsightVideoDownloader.Services
{
    public class PluralsightConnector : IPluralsightConnector
    {
        private readonly IOptions<PluralsightSettings> _pluralsightConfiguration;
        private readonly IHubContext<MyHub> _signalHubContext;

        public PluralsightConnector(IOptions<PluralsightSettings> pluralsightConfiguration,
            IHubContext<MyHub> signalHubContext)
        {
            _pluralsightConfiguration = pluralsightConfiguration;
            _signalHubContext = signalHubContext;
        }

        public List<Module> GetAllMoviesDataFromCourse(string courseUrl)
        {
            List<Module> modulesWithClips;

            try
            {
                _signalHubContext.Clients.All.SendAsync("notification", "Get all movies data...");

                Log.Logger.Information("Get course name...");
                var courseName = PathHelper.GetCourseName(courseUrl);
                Log.Logger.Information($"Course name: {courseName}");
                var url = $"{_pluralsightConfiguration.Value.LearnerPath}{courseName}";
                Log.Logger.Information("Get course data...");
                string skillPathTitle;

                var pluralsightClient = new HttpClient();
                pluralsightClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                pluralsightClient.DefaultRequestHeaders.Add("Host", "app.pluralsight.com");
                pluralsightClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:75.0) Gecko/20100101 Firefox/75.0");
                pluralsightClient.DefaultRequestHeaders.Add("Cookie", _pluralsightConfiguration.Value.Cookie);
                
                using (var response = pluralsightClient.GetAsync(url).Result)
                {
                    using (var content = response.Content)
                    {
                        var result = content.ReadAsStringAsync().Result;
                        var deserializeObject = (JObject)JsonConvert.DeserializeObject(result);
                        var deserializeModules = deserializeObject["modules"];
                        skillPathTitle = deserializeObject["skillPaths"][0]["title"].ToString();
                        Log.Logger.Information("Successfully got modules info...");
                        modulesWithClips = JsonConvert.DeserializeObject<List<Module>>(deserializeModules.ToString());
                    }
                }

                _signalHubContext.Clients.All.SendAsync("notification", "Start getting all movies and transcripts links...");
                
                foreach (var module in modulesWithClips)
                {
                    module.CourseName = courseName;
                    module.SkillPathTitle = skillPathTitle;
                    var moduleIndex = modulesWithClips.IndexOf(module) + 1;
                    module.Title = $"{moduleIndex}. {module.Title}";

                    for (var i = 0; i < module.Clips.Count; i++)
                    {
                        var result = GetVideoAndTranscriptLinksToMovie(module.Clips[i].ClipId, module.Clips[i].Title).Result;
                        module.Clips[i].MovieLink = result.Item1;
                        module.Clips[i].Title = $"{i+1}. {module.Clips[i].Title}";
                        module.Clips[i].TranscriptLink = result.Item2;
                    }
                }

                _signalHubContext.Clients.All.SendAsync("notification", "Successfully got all movies data...");

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

        private async Task<Tuple<string, string>> GetVideoAndTranscriptLinksToMovie(string clipId, string title)
        {
            try
            {
                await _signalHubContext.Clients.All.SendAsync("notification", $"Start extracting movie links for {title}...");
                Log.Logger.Information("Start extracting movie link...");
                Tuple<string, string> linksResult;
 
                var paramJson = "{\"clipId\":\"{clipId}\",\"mediaType\":\"mp4\",\"quality\":\"1280x720\",\"online\":true,\"boundedContext\":\"course\",\"versionId\":\"\"}";
                var paramJsonWithTitle = paramJson.Replace("{clipId}", clipId);
                var stringContent = new StringContent(paramJsonWithTitle, Encoding.UTF8, "application/json");
                var transcriptApi = _pluralsightConfiguration.Value.TranscriptPath;
               
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    client.DefaultRequestHeaders.Add("Host", "app.pluralsight.com");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:75.0) Gecko/20100101 Firefox/75.0");
                    client.DefaultRequestHeaders.Add("Cookie", _pluralsightConfiguration.Value.Cookie);
                    var response = await client.PostAsync(_pluralsightConfiguration.Value.ViewClipPath, stringContent);
                   
                    var result = response.Content.ReadAsStringAsync().Result;
                    var jsonObject = (JObject)JsonConvert.DeserializeObject(result);
                    Log.Logger.Information(jsonObject.ToString());
                    var movieLink = jsonObject["urls"]?[0]?["url"]?.ToString();
                    var version = jsonObject["version"].ToString();
                    var transcriptUrl = transcriptApi.Replace("{clipId}", clipId).Replace("{version}", version);
                    Log.Logger.Information($"Movie link: {movieLink}");
                    await _signalHubContext.Clients.All.SendAsync("notification", $"Successfully got all movie links for {title}...");

                    linksResult = new Tuple<string, string>(movieLink, transcriptUrl);
                }

                await Task.Delay(5000);

                return linksResult;
            }
            catch (Exception e)
            {
                await _signalHubContext.Clients.All.SendAsync("Error");
                Log.Logger.Error(e.ToString());
                Log.Logger.Error(e.StackTrace);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}