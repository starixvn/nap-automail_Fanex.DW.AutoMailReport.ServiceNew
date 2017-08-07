using System;
using System.Collections.Generic;
using System.IO;
using NegativeBalance.Business.Models;
using RazorEngine;
using RazorEngine.Templating;

namespace NegativeBalance.Business.Services
{
    public interface FileService
    {
        string ReadContent(List<Match> matches);
    }

    public class FileServiceImp : FileService
    {
        public string ReadContent(List<Match> matches)
        {
            var result = string.Empty;
            var service = Engine.Razor;

            var matchInfoGroup1 = File.ReadAllText(@"Templates\MatchInfoGroup1.cshtml");
            var matchInfoGroup3 = File.ReadAllText(@"Templates\MatchInfoGroup3.cshtml");
            var matchInfoGroup7 = File.ReadAllText(@"Templates\MatchInfoGroup7.cshtml");
            var matchSettledLogGroup1 = File.ReadAllText(@"Templates\MatchSettledLogGroup1.cshtml");
            var matchSettledLogGroup3 = File.ReadAllText(@"Templates\MatchSettledLogGroup3.cshtml");
            var matchSettledLogGroup7 = File.ReadAllText(@"Templates\MatchSettledLogGroup7.cshtml");
            var listEffectedMembers = File.ReadAllText(@"Templates\ListEffectedMembers.cshtml");
            var mailTemplate = File.ReadAllText(@"Templates\EmailTemplate.cshtml");

            service.AddTemplate("matchInfoGroup1", matchInfoGroup1);
            service.AddTemplate("matchInfoGroup3", matchInfoGroup3);
            service.AddTemplate("matchInfoGroup7", matchInfoGroup7);
            service.AddTemplate("matchSettledLogGroup1", matchSettledLogGroup1);
            service.AddTemplate("matchSettledLogGroup3", matchSettledLogGroup3);
            service.AddTemplate("matchSettledLogGroup7", matchSettledLogGroup7);
            service.AddTemplate("listEffectedMembers", listEffectedMembers);
            service.AddTemplate("mailTemplate", mailTemplate);

            service.Compile("mailTemplate", typeof(List<Match>));
            service.Compile("matchInfoGroup1", typeof(Tuple<int, Match>));
            service.Compile("matchInfoGroup3", typeof(Tuple<int, Match>));
            service.Compile("matchInfoGroup7", typeof(Tuple<int, Match>));
            service.Compile("matchSettledLogGroup1", typeof(Match));
            service.Compile("matchSettledLogGroup3", typeof(Match));
            service.Compile("matchSettledLogGroup7", typeof(Match));
            service.Compile("listEffectedMembers", typeof(Match));

            result = service.Run("mailTemplate", typeof(List<Match>), matches);

            return result;
        }
    }
}