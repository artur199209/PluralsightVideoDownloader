using System.Collections.Generic;

namespace PluralsightVideoDownloader.Models
{
    public class Module
    {
        public string Id { get; set; }
        public string ModuleId { get; set; }
        public string DeprecatedId { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public string PlayerUrl { get; set; }
        public List<Clip> Clips { get; set; }
    }
}