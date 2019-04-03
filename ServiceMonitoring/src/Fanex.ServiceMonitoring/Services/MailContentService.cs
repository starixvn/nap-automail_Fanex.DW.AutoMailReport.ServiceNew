using System;
using System.Collections.Generic;
using System.IO;
using Fanex.ServiceMonitoring.Models;
using RazorEngine;
using RazorEngine.Templating;

namespace Fanex.ServiceMonitoring.Services
{
    public interface MailContentService
    {
        string GenerateMailContent(List<ServiceModel> services);
    }

    public class MailContentServiceImp : MailContentService
    {
        public string GenerateMailContent(List<ServiceModel> services)
        {
            var tuple = new Tuple<List<ServiceModel>>(services);
            var template = File.ReadAllText("Templates/EmailTemplate.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}