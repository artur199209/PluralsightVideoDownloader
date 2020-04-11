﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PluralsightVideoDownloader.Models;
using PluralsightVideoDownloader.Services;
using System.Threading.Tasks;

namespace PluralsightVideoDownloader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<MyHub> _signalHubContext;
        private readonly IPluralsightConnector _pluralsightConnector;
        private readonly IVideoDownloader _videoDownloader;

        public HomeController(IHubContext<MyHub> signalHubContext, IPluralsightConnector pluralsightConnector,
            IVideoDownloader videoDownloader)
        {
            _signalHubContext = signalHubContext;
            _pluralsightConnector = pluralsightConnector;
            _videoDownloader = videoDownloader;
        }
      
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Index2(string url)
        {
            var allMoviesDataFromCourse = _pluralsightConnector.GetAllMoviesDataFromCourse(url);
            _videoDownloader.DownloadAllCourse(allMoviesDataFromCourse).Wait();
            return View("Index");
        }
    }
}