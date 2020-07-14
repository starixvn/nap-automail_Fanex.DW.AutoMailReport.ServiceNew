using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface CsvService
    {
        void Execute<T>(List<T> data, string path);
    }

    public class CsvServiceImp : CsvService
    {
        private readonly Log log;

        public CsvServiceImp(Log log)
        {
            this.log = log;
        }

        public void Execute<T>(List<T> data, string path)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.WriteRecords(data);
                    }
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                log.Log(ex);
            }
        }
    }
}