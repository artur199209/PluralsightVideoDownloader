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

        private static string EnsureNameDoesNotContainSpecialCharacters(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, ' ');
            }

            return name.Trim();
        }

        public static string CombineDownloadPathWithExtension(string mainPath, string skillPathTitle, string courseName, string moduleName, string fileName, string extension)
        {
            var courseNameWithoutSpecialChars = EnsureNameDoesNotContainSpecialCharacters(courseName);
            var moduleNameWithoutSpecialChars = EnsureNameDoesNotContainSpecialCharacters(moduleName);
            var fileNameWithoutSpecialChars = EnsureNameDoesNotContainSpecialCharacters(fileName);

            var mainDirectoryPath = Path.Combine(mainPath, skillPathTitle, courseNameWithoutSpecialChars, moduleNameWithoutSpecialChars);

            if (!Directory.Exists(mainDirectoryPath))
            {
                Directory.CreateDirectory(mainDirectoryPath);
            }

            return Path.Combine(mainDirectoryPath, fileNameWithoutSpecialChars + extension);
        }
    }
}