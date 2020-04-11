using PluralsightVideoDownloader.Models;
using System.Collections.Generic;

namespace PluralsightVideoDownloader.Services
{
    public interface IPluralsightConnector
    {
        List<Module> GetAllMoviesDataFromCourse(string courseUrl);
    }
}