using Fanex.Letstalk.OneTime.Business.Services;
using Newtonsoft.Json;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private readonly DataService dataService;

        public FileServiceImp(DataService dataService)
        {
            this.dataService = dataService;
        }

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
            var folders = dataService.ReadFolderSent()
                .Select(s => s.FolderName)
                .ToList();

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

            dataService.WriteFolderSent(folders);
        }
    }
}