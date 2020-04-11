using PluralsightVideoDownloader.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PluralsightVideoDownloader.Services
{
    public interface IVideoDownloader
    {
        Task DownloadAllCourse(List<Module> modules);
    }
}