using CsvHelper;
using Fanex.MailLetstalkTemplate.Business.Services;
using Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Fanex.MailLetstalkTemplate.Business.Models
{
    public interface CsvService
    {
        string Execute(DataTable data);
    }

    public class CsvServiceImp : CsvService
    {
        private readonly DateRangeServiceProfier dateRangeServiceProfier;
        private readonly MailFormatProvider mailFormatProvider;
        private MailFormat mailFormat;

        public CsvServiceImp(
            DateRangeServiceProfier dateRangeServiceProfier,
            MailFormatProvider mailFormatProvider)
        {
            this.dateRangeServiceProfier = dateRangeServiceProfier;
            this.mailFormatProvider = mailFormatProvider;
        }

        public string Execute(DataTable data)
        {
            mailFormat = mailFormatProvider.Format(AppConfigService.Config.DateRange);
            var date = dateRangeServiceProfier.Date(AppConfigService.Config.DateRange).ToDate;
            var fileName = mailFormat.FileName();
            var path = $@"{AppConfigService.Config.Path}/{date:yyyy}/{date:MM}";
            var localFolder = path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fullName = $@"{path}/{fileName}";

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText($"{fullName}.json", json);

            using (var writer = new StreamWriter($"{fullName}.csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    var rows = data.AsEnumerable();
                    csv.WriteRecords(rows);
                    csv.Dispose();
                }

                writer.Dispose();
            }

            //     return fullName;

            //path = localFolder;
            //var zipFile = $"{path}-{mailFormat.FileName()}.zip";
            //var zip = new FastZip();
            //zip.Password = AppConfigService.Config.Password;
            //zip.CreateZip(zipFile, path, true, string.Empty);

            return fullName;
        }
    }
}