using System;
using System.Collections.Generic;

namespace Fanex.ServiceMonitoring.Services
{
    public interface MessageContentService
    {
        List<string> GenerateMessage();
    }

    public class MessageContentServiceImp : MessageContentService
    {
        private readonly DetectMissingMailService detectMissingMailService;

        public MessageContentServiceImp(DetectMissingMailService detectMissingMailService)
        {
            this.detectMissingMailService = detectMissingMailService;
        }

        public List<string> GenerateMessage()
        {
            var result = new List<string>();
            var content = new List<string>();
            var count = 1;
            var mails = detectMissingMailService.QueryMail();

            foreach (var mail in mails)
            {
                if (mail.Value.Count > 0)
                {
                    content.Add($"{mail.Key}");
                }

                foreach (var c in mail.Value)
                {
                    content.Add($"{count}. {c}");
                    count++;
                }

                count = 1;
                result.Add(string.Join(Environment.NewLine, content));
                content = new List<string>();
            }

            return result;
        }
    }
}