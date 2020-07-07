using System;
using System.IO;

namespace Fanex.TopPunterCasino.Business.Services
{
    public interface FileService
    {
        void MoveFiles(string source, string destination);
    }

    public class FileServiceImp : FileService
    {
        public void MoveFiles(string source, string destination)
        {
            DirectoryMove(source, destination, true);
            DeleteEmptyDirectoryOlderThanOneDay(source);
        }

        private void DirectoryMove(string source, string destination, bool includeSubDirectory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(source);

            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found or need a permission to access on this: " + source);
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            var files = directoryInfo.GetFiles("*.*");
            foreach (var file in files)
            {
                var destinationFile = Path.Combine(destination, file.Name);

                SafeMoveFile(file, destinationFile);
            }

            if (includeSubDirectory)
            {
                var subSourceDirectories = directoryInfo.GetDirectories();
                foreach (var subSourceDirectory in subSourceDirectories)
                {
                    string subDestinationDirectory = Path.Combine(destination, subSourceDirectory.Name);
                    DirectoryMove(subSourceDirectory.FullName, subDestinationDirectory, includeSubDirectory);
                }
            }
        }

        private static void SafeMoveFile(FileInfo file, string destinationFile)
        {
            if (!File.Exists(destinationFile)
                || file.LastWriteTime > (new FileInfo(destinationFile)).LastWriteTime)
            {
                file.CopyTo(destinationFile, true);
            }

            try
            {
                var ageOfFile = (DateTime.Now - file.LastWriteTime).TotalHours;
                if (ageOfFile >= 25)
                {
                    file.Delete();
                }
            }
            catch (IOException)
            {
                // File is in use
            }
        }

        private static void DeleteEmptyDirectoryOlderThanOneDay(string location)
        {
            foreach (var directory in Directory.GetDirectories(location))
            {
                DeleteEmptyDirectoryOlderThanOneDay(directory);
                var createdDate = Directory.GetCreationTime(directory);
                var isOld = createdDate <= DateTime.Now.AddDays(-1);
                if (isOld
                    && Directory.GetFiles(directory).Length == 0
                    && Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}