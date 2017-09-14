using System;
using System.Collections.Generic;
using System.IO;
using MaxLossControl.CustomerMovement.Business.Models;
using RazorLight;

namespace MaxLossControl.CustomerMovement.Business.Services
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

            var root = $@"{Directory.GetCurrentDirectory()}\Templates";
            var engine = EngineFactory.CreatePhysical(root);
            result = engine.Parse("EmailTemplate.cshtml", tuple);

            return result;
        }
    }
}