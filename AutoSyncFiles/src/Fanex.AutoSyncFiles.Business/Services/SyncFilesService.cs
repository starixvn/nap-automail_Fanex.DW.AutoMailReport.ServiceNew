using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Fanex.AutoSyncFiles.Business.Services
{
    using System.Linq;
    using Models;

    public interface SyncFilesService
    {
        void Execute();
    }

    public class SyncFilesServiceImp : SyncFilesService
    {
        private readonly Log log;

        private readonly List<string> messages = new List<string>
        {
            "Unable to read data from the transport connection",
            "Unable to write data to the transport connection:",
            "Timed out trying to read data from the socket stream!",
            "Unable to identify the local data socket: Address already in use",
            "Cannot download non-existant directory",
            "Could not find a part of the path"
        };

        public SyncFilesServiceImp(Log log)
        {
            this.log = log;
        }

        public void Execute()
        {
            try
            {
                FtpDownload();
            }
            catch (Exception e)
            {
                log.Log(e);
            }
        }

        private void FtpDownload()
        {
            using (var ftpClient = CreateFtpClient())
            {
                FtpDownloadByFtpClient(ftpClient);
            }

            GC.Collect();
        }

        private void FtpDownloadByFtpClient(IFtpClient ftpClient)
        {
            foreach (var path in AppConfigService.AppConfig.Paths)
            {
                TryDownloadFtpDirectory(ftpClient, path);
            }
        }

        private void TryDownloadFtpDirectory(IFtpClient ftpClient, PathModel path)
        {
            var currentFolder = string.Empty;
            try
            {
                if (!ftpClient.IsConnected)
                {
                    ftpClient.Connect();
                }

                Directory.CreateDirectory(path.Destination);
                var result = ftpClient.DownloadDirectory(path.Destination, path.Source);

                currentFolder = result
                    .Where(f => f.Type == FtpFileSystemObjectType.File)
                    .Select(f => f.RemotePath)
                    .LastOrDefault();
            }
            catch (Exception e)
            {
                var isIgnoredLog = messages.Any(m => e.Message.Contains(m));
                if (!isIgnoredLog)
                {
                    log.Log($"Fanex.AutoSyncFiles.Service: {currentFolder} - {e.Message}");
                }
            }
        }

        private static FtpClient CreateFtpClient()
        {
           return new FtpClient(AppConfigService.AppConfig.FTPConfig.Host)
            {
                Credentials = new NetworkCredential(AppConfigService.AppConfig.FTPConfig.UserName, AppConfigService.AppConfig.FTPConfig.Password),
                Port = AppConfigService.AppConfig.FTPConfig.Port
            };
        }
    }
}