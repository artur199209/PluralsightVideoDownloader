using Microsoft.AspNetCore.Mvc;
using PluralsightVideoDownloader.Services;

namespace PluralsightVideoDownloader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPluralsightConnector _pluralsightConnector;
        private readonly IVideoDownloader _videoDownloader;

        public HomeController(IPluralsightConnector pluralsightConnector,
            IVideoDownloader videoDownloader)
        {
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