using CsvHelper;
using System.Collections.Generic;
using System.IO;

namespace Fanex.ESportDataMining.Business.Services
{
    public interface CsvService
    {
        void Execute<T>(List<T> data, string path);
    }

    public class CsvServiceImp : CsvService
    {
        public void Execute<T>(List<T> data, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(data);
                }
            }
        }
    }
}