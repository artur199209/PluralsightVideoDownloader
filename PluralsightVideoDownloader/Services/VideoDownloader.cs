using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PluralsightVideoDownloader.Helpers;
using PluralsightVideoDownloader.Models;
using Serilog;

namespace PluralsightVideoDownloader.Services
{
    public class VideoDownloader
    {
        private readonly string _downloadPath;

        public VideoDownloader(string downloadPath)
        {
            _downloadPath = downloadPath;
        }

        private async Task DownloadModule(Module module)
        {
            foreach (var clip in module.Clips)
            {
                var movieDownloadPath = PathHelper.CombineDownloadPathWithExtension(_downloadPath, module.CourseName, 
                    module.Title, clip.Title, ".mp4");
                var transcriptDownloadPath = PathHelper.CombineDownloadPathWithExtension(_downloadPath, module.CourseName,
                    module.Title, clip.Title, ".txt");
                await DownloadVideoAsync(clip.MovieLink, clip.TranscriptLink, movieDownloadPath, transcriptDownloadPath);
            }
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
                Log.Logger.Error(e.ToString());
                Log.Logger.Error(e.StackTrace);
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task DownloadAllCourse(List<Module> modules)
        {
            await Task.WhenAll(modules.Select(DownloadModule));
        }

    }
}