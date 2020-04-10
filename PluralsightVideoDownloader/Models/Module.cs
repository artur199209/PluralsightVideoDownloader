using System.Collections.Generic;

namespace PluralsightVideoDownloader.Models
{
    public class Module
    {
        public string Id { get; set; }
        public string CourseName { get; set; }
        public string Title { get; set; }
        public List<Clip> Clips { get; set; }
    }
}