using Microsoft.AspNetCore.Mvc;
using PluralsightVideoDownloader.Services;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index2(string url)
        {
            var allMoviesDataFromCourse =  _pluralsightConnector.GetAllMoviesDataFromCourse(url);
            await _videoDownloader.DownloadAllCourse(allMoviesDataFromCourse);
            return View("Index");
        }
    }
}