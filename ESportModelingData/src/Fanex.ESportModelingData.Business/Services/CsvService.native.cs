using System.Collections.Generic;
using System.IO;
using CsvHelper;

namespace Fanex.ESportModelingData.Business.Services
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