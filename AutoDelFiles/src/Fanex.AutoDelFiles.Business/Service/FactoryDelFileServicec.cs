using Fanex.AutoDelFiles.Business.Models;
using System.Collections.Generic;

namespace Fanex.AutoDelFiles.Business.Service
{
    public interface FactoryDelFileService
    {
        FileService DelFiles(PathModel path);
    }

    public class FactoryDelFileServiceImp : FactoryDelFileService
    {
        private readonly DelFileService delFileService;

        public FactoryDelFileServiceImp(DelFileService delFileService)
        {
            this.delFileService = delFileService;
        }

        public FileService DelFiles(PathModel path)
        {
            var fileService = new Dictionary<bool, FileService>
            {
                { true, new AuthorizedFileService(delFileService) },
                { false, new UnAuthorizedFileService(delFileService) }
            };

            var isAuthorized = !string.IsNullOrEmpty(path.SourceServerUsername);
            return fileService[isAuthorized];
        }
    }
}