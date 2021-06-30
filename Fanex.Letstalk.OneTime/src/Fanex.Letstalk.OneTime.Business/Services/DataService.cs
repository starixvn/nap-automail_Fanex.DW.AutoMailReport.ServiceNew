using Fanex.Data.Repository;
using Fanex.Letstalk.OneTime.Business.Criterias;
using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Fanex.Letstalk.OneTime.Business.Services
{
    public interface DataService
    {
        List<MailModel> QueryMail(DateTime rptDate, JobStatus jobStatus);

        List<OneTimeModel> QueryReportPreRunLog();

        List<OneTimeModel> ReadFolderSent();

        void WriteFolderSent(List<string> folders);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateService dateService;

        public DataServiceImp(IDynamicRepository repository, DateService dateService)
        {
            this.repository = repository;
            this.dateService = dateService;
        }

        public List<OneTimeModel> QueryReportPreRunLog()
        {
            var date = dateService.Date;
            var criteria = new LetstalkPreRunLogSelectCriteria
            {
                Date = $"{date:yyyyMMdd}",
                IsSent = false
            };

            return repository.Fetch<OneTimeModel>(criteria).ToList();
        }

        public List<MailModel> QueryMail(DateTime rptDate, JobStatus jobStatus)
        {
            var criteria = new CheckMailStatusCriteria
            {
                FromDate = rptDate,
                ToDate = rptDate,
                Status = (int)jobStatus
            };
            return repository.Fetch<MailModel>(criteria).ToList();
        }

        public List<OneTimeModel> ReadFolderSent()
        {
            var date = dateService.Date;
            var criteria = new LetstalkPreRunLogSelectCriteria
            {
                Date = $"{date:yyyyMMdd}",
                IsSent = true
            };

            return repository.Fetch<OneTimeModel>(criteria).ToList();
        }

        public void WriteFolderSent(List<string> folders)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("notify");
            var row = doc.CreateElement("row");

            folders.ForEach(folderName =>
            {
                row.SetAttribute("FolderName", folderName);
                row.SetAttribute("Date", $"{dateService.Date:yyyyMMdd}");
                root.AppendChild(row);
            });

            doc.AppendChild(root);

            var xmlFolders = doc.InnerXml;
            var criteria = new LetstalkPreRunLogUpdateCriteria
            {
                FolderSent = xmlFolders
            };

            repository.Execute(criteria);
        }
    }
}