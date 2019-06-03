using System;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace Fanex.BetBuilder.Business.Services
{
    public interface MailContentService
    {
        string GenerateDailyMailContent(DateTime winlostDate, bool isNoData);

        string GenerateMonthlyMailContent(DateTime fromDate, DateTime toDate, bool isNoData);
    }

    public class MailContentServiceImp : MailContentService
    {
        public string GenerateDailyMailContent(DateTime winlostDate, bool isNoData)
        {
            var tuple = new Tuple<DateTime, bool>(winlostDate, isNoData);
            var template = File.ReadAllText("Templates/DailyEmailTemplate.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }

        public string GenerateMonthlyMailContent(DateTime fromDate, DateTime toDate, bool isNoData)
        {
            var tuple = new Tuple<DateTime, DateTime, bool>(fromDate, toDate, isNoData);
            var template = File.ReadAllText("Templates/MonthlyEmailTemplate.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}