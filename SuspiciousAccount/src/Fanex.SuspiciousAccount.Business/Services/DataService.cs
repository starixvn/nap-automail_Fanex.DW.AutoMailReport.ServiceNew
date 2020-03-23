using Fanex.Data.Repository;
using Fanex.SuspiciousAccount.Business.Criterias;
using Fanex.SuspiciousAccount.Business.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System;

namespace Fanex.SuspiciousAccount.Business.Services
{
    public interface DataService
    {
        List<AccountModel> QueryAccount();

        void UpdateAccount(List<AccountModel> accounts);

        void CheckIn(ServiceMonitoringCheckInCriteria criteria);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        

        public DataServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;            
        }

        public void CheckIn(ServiceMonitoringCheckInCriteria criteria)
        {
            repository.Execute(criteria);
        }

        public List<AccountModel> QueryAccount()
        {
            var criteria = new SelectAccountCriteria();
            return repository.Fetch<AccountModel>(criteria).ToList();            
        }

        public void UpdateAccount(List<AccountModel> accounts)
        {
            var memNotifyXml = GenerateXML(accounts);

            var criteria = new UpdateIsNotifiedAccount { MemNotify = memNotifyXml };
            repository.Execute(criteria);
        }

        private string GenerateXML(List<AccountModel> accounts)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("notify");
            accounts.ForEach(member =>
            {
                var row = doc.CreateElement("row");
                row.SetAttribute("custid", member.CustId.ToString());
                row.SetAttribute("NotifyStatus1", member.NotifyStatus1.ToString());
                row.SetAttribute("NotifyStatus2", member.NotifyStatus2.ToString());
                root.AppendChild(row);
            });

            doc.AppendChild(root);
            return doc.InnerXml;
        }
    }
}