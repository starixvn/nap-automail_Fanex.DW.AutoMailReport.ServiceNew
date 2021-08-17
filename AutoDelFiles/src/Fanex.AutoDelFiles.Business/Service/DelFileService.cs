using System;
using System.IO;

namespace Fanex.AutoDelFiles.Business.Service
{
    public interface DelFileService
    {
        void DelFiles(string source, int maxAgeOfFileInDays);
    }

    public class DelFileServiceImp : DelFileService
    {
        private readonly string searchPattern = "*.*";

        public void DelFiles(string source, int maxAgeOfFileInDays)
        {
            DeleteEmptyDirectory(source);
            DirectoryDelete(source, maxAgeOfFileInDays, true);
        }

        private void DirectoryDelete(string source, int maxAgeOfFileInDays, bool includeSubDirectory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(source);

            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found or need a permission to access on this: " + source);
            }

            var files = directoryInfo.GetFiles(searchPattern);
            foreach (var file in files)
            {
                SafeDelete(file, maxAgeOfFileInDays);
            }

            if (includeSubDirectory)
            {
                var subSourceDirectories = directoryInfo.GetDirectories();
                foreach (var subSourceDirectory in subSourceDirectories)
                {
                    string subSource = Path.Combine(source, subSourceDirectory.Name);
                    DirectoryDelete(subSource, maxAgeOfFileInDays, includeSubDirectory);
                }
            }
        }

        private static void SafeDelete(FileInfo file, int maxAgeOfFileInDays)
        {
            var ageOfFile = (DateTime.Now - file.LastWriteTime).TotalDays;
            if (ageOfFile >= maxAgeOfFileInDays)
            {
                Console.WriteLine(file.FullName);
                file.Delete();
            }
        }

        private static void DeleteEmptyDirectory(string location)
        {
            foreach (var directory in Directory.GetDirectories(location))
            {
                DeleteEmptyDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0
                    && Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}