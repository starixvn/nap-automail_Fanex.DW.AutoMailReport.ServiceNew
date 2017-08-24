using System;
using System.Collections.Generic;
using System.IO;
using CustomerMovement.Business.Models;
using RazorEngine;
using RazorEngine.Templating;

namespace CustomerMovement.Business.Services
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
            var service = Engine.Razor;

            var mailTemplate = File.ReadAllText(@"Templates\EmailTemplate.cshtml");
            var groupHeadline = File.ReadAllText(@"Templates\GroupHeadline.cshtml");
            var customerMovement = File.ReadAllText(@"Templates\CustomerMovement.cshtml");
            var directCustomer = File.ReadAllText(@"Templates\DirectCustomers.cshtml");

            service.AddTemplate("mailTemplate", mailTemplate);
            service.AddTemplate("groupHeadline", groupHeadline);
            service.AddTemplate("customerMovement", customerMovement);
            service.AddTemplate("directCustomer", directCustomer);

            service.Compile("mailTemplate", typeof(Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>, DateService>));
            service.Compile("groupHeadline", typeof(IEnumerable<GroupHeadline>));
            service.Compile("customerMovement", typeof(IEnumerable<Movement>));
            service.Compile("directCustomer", typeof(IEnumerable<DirectCustomer>));

            var tuple = new Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>, DateService>(data.Item1, data.Item2, data.Item3, date);
            result = service.Run("mailTemplate", typeof(Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>, DateService>), tuple);

            return result;
        }
    }
}