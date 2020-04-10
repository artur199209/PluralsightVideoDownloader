using System.IO;
using System.Linq;

namespace PluralsightVideoDownloader.Helpers
{
    public static class PathHelper
    {
        public static string GetCourseName(string path)
        {
            var segments = path.Split('/').ToList();
            return segments[segments.Count - 2];
        }

        public static string CombineDownloadPathWithExtension(string mainPath, string courseName, string moduleName, string fileName, string extension)
        {
            var pathWithoutExtension = Path.Combine(mainPath, courseName, moduleName);

            if (!Directory.Exists(pathWithoutExtension))
            {
                Directory.CreateDirectory(pathWithoutExtension);
            }

            return Path.Combine(pathWithoutExtension, fileName + extension);
        }
    }
}