using System;
using System.Collections.Generic;
using System.IO;
using MaxLossMonitoring.Business.Models;
using RazorEngine;
using RazorEngine.Templating;

namespace MaxLossMonitoring.Business.Services
{
    public interface FileService
    {
        string ReadContentWithAttachedFile(Tuple<List<MonitorList>, List<MonitorListRaw>> data);

        string ReadContentWithoutAttachedFile(string fileName);
    }

    public class FileServiceImp : FileService
    {
        private readonly ApplicationConfigurationService appConfig;

        public FileServiceImp(ApplicationConfigurationService appConfig)
        {
            this.appConfig = appConfig;
        }

        public string ReadContentWithAttachedFile(Tuple<List<MonitorList>, List<MonitorListRaw>> data)
        {
            var service = Engine.Razor;

            var emailTemplate = File.ReadAllText($@"Templates\EmailTemplate.cshtml");
            var emailRowTemplate = File.ReadAllText($@"Templates\EmailRowTemplate.cshtml");

            service.AddTemplate("emailTemplate", emailTemplate);
            service.AddTemplate("emailRowTemplate", emailRowTemplate);

            var tuple = new Tuple<List<MonitorList>, List<MonitorListRaw>, string>(data.Item1, data.Item2, appConfig.Configurations.NapReportURL);
            service.Compile("emailTemplate", typeof(Tuple<List<MonitorList>, List<MonitorListRaw>, string>));
            service.Compile("emailRowTemplate", typeof(Tuple<MonitorListRaw, int, bool>));

            var result = Engine.Razor.RunCompile(emailTemplate, emailTemplate, tuple.GetType(), tuple);

            return result;
        }

        public string ReadContentWithoutAttachedFile(string fileName)
        {
            var service = Engine.Razor;
            var notificationEmail = File.ReadAllText($@"Templates\NotificationEmail.cshtml");
            service.AddTemplate("notificationEmail", notificationEmail);
            var tuple = new Tuple<string, string>(fileName, appConfig.Configurations.NapReportURL);
            service.Compile("notificationEmail", typeof(Tuple<string, string>));

            var result = Engine.Razor.RunCompile(notificationEmail, notificationEmail, tuple.GetType(), tuple);

            return result;
        }
    }
}