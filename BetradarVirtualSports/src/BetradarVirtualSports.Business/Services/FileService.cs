using System;
using System.IO;
using Fanex.Service.Core;
using RazorEngine;
using RazorEngine.Templating;

namespace BetradarVirtualSports.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsNotReady();

        string ReadContentWhenDataIsAvailable();
    }

    public class FileServiceImp : FileService
    {
        public string ReadContentWhenDataIsAvailable()
        {
            var templatePath = $@"Templates\ContentWithAttachment.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var todate = $"{SystemTime.Now:MM/dd/yyyy}";
            return Engine.Razor.RunCompile(emailTemplate, templatePath, todate.GetType(), todate);
        }

        public string ReadContentWhenDataIsNotReady()
        {
            throw new NotImplementedException();
        }
    }
}