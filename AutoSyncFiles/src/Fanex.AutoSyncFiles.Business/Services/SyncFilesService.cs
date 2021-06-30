using Fanex.MailDelivery.Core.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Fanex.AutoSyncFiles.Business.Services
{
    public interface SyncFilesService
    {
        JobMessage Execute();
    }

    public class SyncFilesServiceImp : SyncFilesService
    {
        private readonly Log log;

        private FtpClient ftpClient = new FtpClient(AppConfigService.AppConfig.FTPConfig.Host)
        {
            Credentials = new NetworkCredential(AppConfigService.AppConfig.FTPConfig.UserName, AppConfigService.AppConfig.FTPConfig.Password),
            Port = AppConfigService.AppConfig.FTPConfig.Port
        };

        public SyncFilesServiceImp(Log log)
        {
            this.log = log;
        }

        public JobMessage Execute()
        {
            var downloadedFiles = new List<string>();
            var jobStatus = JobStatus.DataIsNotReady;
            var jobMessage = new JobMessage
            {
                JobStatus = jobStatus,
                Mail = new Mail { Subject = "Auto Sync Files", Body = "Sysc file service is processing ..." },
            };

            try
            {
                if (AppConfigService.IsProcessing)
                {
                    return jobMessage;
                }

                AppConfigService.IsProcessing = true;
                FTPDownloadFiles(downloadedFiles);
                jobStatus = JobStatus.Success;
            }
            catch (Exception e)
            {
                jobStatus = JobStatus.JobFailed;
                log.Log(e);
                downloadedFiles.Add(e.Message);
            }
            finally
            {
                if (ftpClient != null)
                {
                    ftpClient.Disconnect();
                }

                AppConfigService.IsProcessing = false;
                jobMessage.JobStatus = jobStatus;
                jobMessage.Mail = new Mail
                {
                    Subject = "Auto Sync Files",
                    Body = string.Join(Environment.NewLine, downloadedFiles)
                };
            }

            return jobMessage;
        }

        private void FTPDownloadFiles(List<string> downloadedFiles)
        {
            ftpClient.Connect();
            ftpClient.ConnectTimeout = 3600;
            var utcNow = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);

            foreach (var path in AppConfigService.AppConfig.Paths)
            {
                Directory.CreateDirectory(path.Destination);
                var result = ftpClient.DownloadDirectory(path.Destination, path.Source, FtpFolderSyncMode.Update);
                result.ForEach(file =>
                {
                    if (file.Type == FtpFileSystemObjectType.File)
                    {
                        var files = ftpClient.GetListing(file.RemotePath);
                        foreach (FtpListItem item in files)
                        {
                            downloadedFiles.Add(item.FullName);
                        }
                    }
                });
            }
        }
    }
}