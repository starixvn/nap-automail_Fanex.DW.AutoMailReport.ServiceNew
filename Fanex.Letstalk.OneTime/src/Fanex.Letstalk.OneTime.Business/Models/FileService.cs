using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.Letstalk.OneTime.Business.Models
{
    public interface FileService
    {
        string LoadMessage(bool isMail);

        string LoadMailTemplate(List<string> fileNames);
    }

    public class FileServiceImp : FileService
    {
        public string LoadMailTemplate(List<string> fileNames)
        {
            var templatePath = @"Templates\ContentWithAttachment.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var result = Engine.Razor.RunCompile(emailTemplate, $"template{DateTime.Now.Ticks}", model: fileNames);
            return result;
        }

        public string LoadMessage(bool isMail)
        {
            if (isMail)
            {
                return File.ReadAllText("Templates/EmailTemplate.cshtml");
            }
            else
            {
                return File.ReadAllText("Templates/Message.txt");
            }
        }
    }
}