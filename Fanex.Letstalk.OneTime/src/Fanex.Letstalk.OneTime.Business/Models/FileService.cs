using Fanex.Letstalk.OneTime.Business.Services;
using Newtonsoft.Json;
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

        void WriteFolderSent(List<string> folders);

        List<string> ReadFolderSent();
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

        public List<string> ReadFolderSent()
        {
            var folders = new List<string>();
            var fileName = "FolderReadySent.json";
            var path = $@"{AppConfigService.AppConfig.Path}/{fileName}";
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);
                var oldFolders = JsonConvert.DeserializeObject<List<string>>(text);
                folders.AddRange(oldFolders);
            }

            return folders;
        }

        public void WriteFolderSent(List<string> folders)
        {
            var fileName = "FolderReadySent.json";
            var path = $@"{AppConfigService.AppConfig.Path}/{fileName}";
            var oldFolders = ReadFolderSent();
            folders.AddRange(oldFolders);
            var json = JsonConvert.SerializeObject(folders, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}