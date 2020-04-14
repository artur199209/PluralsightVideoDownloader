using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PluralsightVideoDownloader.Helpers;
using PluralsightVideoDownloader.Models;
using Serilog;

namespace PluralsightVideoDownloader.Services
{
    public class VideoDownloader : IVideoDownloader
    {
        private readonly IHubContext<MyHub> _signalHubContext;
        private readonly IOptions<PluralsightSettings> _pluralsightConfiguration;

        public VideoDownloader(IHubContext<MyHub> signalHubContext, 
            IOptions<PluralsightSettings> pluralsightConfiguration)
        {
            _signalHubContext = signalHubContext;
            _pluralsightConfiguration = pluralsightConfiguration;
        }

        private async Task DownloadModule(Module module)
        {
            await _signalHubContext.Clients.All.SendAsync("notification", $"{module.Title} is being downloaded...");

            foreach (var clip in module.Clips)
            {
                var movieDownloadPath = PathHelper.CombineDownloadPathWithExtension(_pluralsightConfiguration.Value.DownloadPath, module.SkillPathTitle, module.CourseName,
                    module.Title, clip.Title, ".mp4");
                var transcriptDownloadPath = PathHelper.CombineDownloadPathWithExtension(_pluralsightConfiguration.Value.DownloadPath, module.SkillPathTitle, module.CourseName,
                    module.Title, clip.Title, ".txt");
                await DownloadVideoAsync(clip.MovieLink, clip.TranscriptLink, movieDownloadPath, transcriptDownloadPath);

                var progress = CalculateDownloadVideoProgress(module.Clips.IndexOf(clip) + 1, module.Clips.Count);
                await _signalHubContext.Clients.All.SendAsync("UpdateProgressBar", module.Title, progress);
            }

            await _signalHubContext.Clients.All.SendAsync("notification", $"{module.Title} has been downloaded...");

        }

        private async Task DownloadVideoAsync(string movieLink, string transcriptLink, string movieDownloadPath, string transcriptDownloadPath)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    Log.Logger.Information($"Downloading movie: {movieLink} to path: {movieDownloadPath}");
                    await webClient.DownloadFileTaskAsync(new Uri(movieLink), movieDownloadPath);
                    Log.Logger.Information($"Downloading transcript: {transcriptLink} to path: {transcriptDownloadPath}");
                    await webClient.DownloadFileTaskAsync(new Uri(transcriptLink), transcriptDownloadPath);
                }
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

        public async Task DownloadAllCourse(List<Module> modules)
        {
            var modulesArray = modules.Select(x => x.Title).ToArray();
            await _signalHubContext.Clients.All.SendAsync("InitProgressBar", modulesArray);

            await Task.WhenAll(modules.Select(DownloadModule));
            await _signalHubContext.Clients.All.SendAsync("Complete");
        }

        private int CalculateDownloadVideoProgress(int completed, int all)
        {
            int progress = 100 * completed / all;
            return progress;
        }

    }
}