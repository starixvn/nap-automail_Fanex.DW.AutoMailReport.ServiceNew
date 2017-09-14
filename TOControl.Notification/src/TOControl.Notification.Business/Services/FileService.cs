using System;
using System.Collections.Generic;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using TOControl.Notification.Business.Models;

namespace TOControl.Notification.Business.Services
{
    public interface FileService
    {
        string ReadContent(Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> data, DateService date);
    }

    public class FileServiceImp : FileService
    {
        public string ReadContent(Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> data, DateService date)
        {
            var result = string.Empty;
            var tuple = new Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>, DateService>(data.Item1, data.Item2, data.Item3, date);

            var service = Engine.Razor;
            var emailTemplate = File.ReadAllText(@"Templates\EmailTemplate.cshtml");
            var groupHeadline = File.ReadAllText(@"Templates\GroupHeadline.cshtml");
            var customerMovement = File.ReadAllText(@"Templates\CustomerMovement.cshtml");
            var directCustomers = File.ReadAllText(@"Templates\DirectCustomers.cshtml");
            
            service.AddTemplate("groupHeadline", groupHeadline);
            service.AddTemplate("customerMovement", customerMovement);
            service.AddTemplate("directCustomers", directCustomers);
            service.AddTemplate("emailTemplate", emailTemplate);
            
            service.Compile("groupHeadline", typeof(IEnumerable<GroupHeadline>));
            service.Compile("customerMovement", typeof(IEnumerable<Movement>));
            service.Compile("directCustomers", typeof(IEnumerable<DirectCustomer>));
            service.Compile("emailTemplate", typeof(Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>, DateService>));

            result = service.Run("emailTemplate", typeof(Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>, DateService>), tuple);

            return result;
        }
    }
}