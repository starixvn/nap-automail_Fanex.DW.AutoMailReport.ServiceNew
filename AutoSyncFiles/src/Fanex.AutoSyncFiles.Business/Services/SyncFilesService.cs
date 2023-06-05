using Fanex.MailDelivery.Core.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Fanex.AutoSyncFiles.Business.Services
{
    using System.Linq;

    public interface SyncFilesService
    {
        JobMessage Execute();
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

        public JobMessage Execute()
        {
            var jobStatus = JobStatus.DataIsNotReady;
            var jobMessage = new JobMessage
            {
                JobStatus = jobStatus,
                Mail = new Mail { Subject = "Auto Sync Files", Body = "Sync file service is processing ..." }
            };

            try
            {
                if (AppConfigService.IsProcessing)
                {
                    return jobMessage;
                }

               
                AppConfigService.IsProcessing = true;

                FtpDownloadFiles();
                jobStatus = JobStatus.Success;
            }
            catch (Exception e)
            {
                jobStatus = JobStatus.JobFailed;
                log.Log(e);
            }
            finally
            {
                AppConfigService.IsProcessing = false;
                jobMessage.JobStatus = jobStatus;
                jobMessage.Mail = new Mail
                {
                    Subject = "Auto Sync Files",
                    Body = "Auto Sync Files"
                };
            }

            return jobMessage;
        }

        private void FtpDownloadFiles()
        {
            using (var ftpClient = CreateFtpClient())
            {
                FtpDownloadFilesByFtpClient(ftpClient);
            }

            GC.Collect();
        }

        private void FtpDownloadFilesByFtpClient(FtpClient ftpClient)
        {
            var currentFolder = string.Empty;

            foreach (var path in AppConfigService.AppConfig.Paths)
            {
                try
                {
                    if (!ftpClient.IsConnected)
                    {
                        ftpClient.Connect();
                    }

                    Directory.CreateDirectory(path.Destination);
                    var result = ftpClient.DownloadDirectory(path.Destination, path.Source);

                    result.ForEach(file =>
                    {
                        if (file.Type == FtpObjectType.File)
                        {
                            currentFolder = file.RemotePath;
                        }
                    });
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
        }

        private FtpClient CreateFtpClient()
        {
           return new FtpClient(AppConfigService.AppConfig.FTPConfig.Host)
            {
                Credentials = new NetworkCredential(AppConfigService.AppConfig.FTPConfig.UserName, AppConfigService.AppConfig.FTPConfig.Password),
                Port = AppConfigService.AppConfig.FTPConfig.Port
            };
        }
    }
}