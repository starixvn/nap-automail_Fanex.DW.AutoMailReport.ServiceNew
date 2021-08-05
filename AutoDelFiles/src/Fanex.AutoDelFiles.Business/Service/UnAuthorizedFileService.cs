using Fanex.AutoDelFiles.Business.Models;

namespace Fanex.AutoDelFiles.Business.Service
{
    public class UnAuthorizedFileService : FileService
    {
        private readonly DelFileService delFileService;

        public UnAuthorizedFileService(DelFileService delFileService)
        {
            this.delFileService = delFileService;
        }

        public void Execute(PathModel path)
        {
            delFileService.DelFiles(path.Source, path.MaxAgeOfFileInDays);
        }
    }
}