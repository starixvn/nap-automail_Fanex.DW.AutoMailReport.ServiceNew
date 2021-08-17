using Fanex.AutoDelFiles.Business.Models;
using Fanex.Common;
using System;
using System.Net;
using WNet.Connection;

namespace Fanex.AutoDelFiles.Business.Service
{
    public class AuthorizedFileService : FileService
    {
        private readonly DelFileService delFileService;

        public AuthorizedFileService(DelFileService delFileService)
        {
            this.delFileService = delFileService;
        }

        public void Execute(PathModel path)
        {
            var pwd = Cryptography.CfsDeCrypt(path.SourceServerPassword);
            var networkCredential = new NetworkCredential(path.SourceServerUsername, pwd);
            using (new NetworkConnection(path.Source, networkCredential))
            {
                delFileService.DelFiles(path.Source, path.MaxAgeOfFileInDays);
            }
        }
    }
}