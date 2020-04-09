
namespace PluralsightVideoDownloader.Models
{
    public class Movie
    {
        public string Id { get; set; }
        public string ClipId { get; set; }
        public string DeprecateId { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public string ModuleTitle { get; set; }
        public bool Trailer { get; set; }
        public string PlayerUrl { get; set; }
    }
}