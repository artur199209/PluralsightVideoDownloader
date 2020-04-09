using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PluralsightVideoDownloader.Services
{
    public class VideoDownloader
    {
        public void DownloadVideo(string movieLink)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileAsync(
                    new System.Uri(movieLink), @"D:/aa.mp4");
            }
        }

        public void DownloadAllCourse()
        {

        }

    }
}