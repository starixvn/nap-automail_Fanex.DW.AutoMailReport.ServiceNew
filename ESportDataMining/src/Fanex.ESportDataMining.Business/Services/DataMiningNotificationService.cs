using Fanex.ESportDataMining.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.ESportDataMining.Business.Services
{
    public interface DataMiningNotificationService
    {
        JobMessage Execute();
    }

    public class DataMiningNotificationServiceImp : DataMiningNotificationService
    {
        private readonly Log log;
        private readonly RepositoryService repositoryService;
        private readonly ExportService exportService;

        public DataMiningNotificationServiceImp(Log log, RepositoryService repositoryService, ExportService exportService)
        {
            this.log = log;
            this.repositoryService = repositoryService;
            this.exportService = exportService;
        }

        public JobMessage Execute()
        {
            var result = new ExecuteResultModel();
            JobMessage jobMessage = new JobMessage();
            try
            {
                result = SendNotification();
            }
            catch (Exception e)
            {
                result.Success = false;

                result.Message = e.Message;
                log.Log(e);
            }
            finally
            {
                jobMessage.Mail = result.Mail;
                jobMessage.JobStatus = result.Success ? JobStatus.SuccessWithoutSendmail : JobStatus.DataIsNotReady;
                jobMessage.TaskLog = new TaskLog
                {
                    LogTime = DateTime.Now,
                    Message = result.Message
                };
            }

            return jobMessage;
        }

        private ExecuteResultModel SendNotification()
        {
            var bettype = repositoryService.QueryBettype();
            var custESport = repositoryService.QueryCustESport();
            var liveStatus = repositoryService.QueryLiveStatus();
            var currency = repositoryService.QueryCurrency();
            var site = repositoryService.QuerySite();
            var match = repositoryService.QueryMatch();
            var date = repositoryService.QueryDate();
            var league = repositoryService.QueryLeague();
            var customer = repositoryService.QueryCustomer();
            var trader = repositoryService.QueryTrader();

            exportService.Export(bettype, "NAP_DW_Esport_Dim_Bettype.csv");
            exportService.Export(custESport, "NAP_DW_Esport_FactCustEsport.csv");
            exportService.Export(liveStatus, "NAP_DW_Esport_Dim_LiveStatus.csv");
            exportService.Export(currency, "NAP_DW_Esport_Dim_Currency.csv");
            exportService.Export(site, "NAP_DW_Esport_Dim_Site.csv");
            exportService.Export(match, "NAP_DW_Esport_Dim_Match.csv");
            exportService.Export(date, "NAP_DW_Esport_Dim_Date.csv");
            exportService.Export(league, "NAP_DW_Esport_Dim_League.csv");
            exportService.Export(customer, "NAP_DW_Esport_Dim_Customer.csv");
            exportService.Export(trader, "NAP_DW_Esport_Dim_Trader.csv");

            exportService.CleanUpFiles();

            return new ExecuteResultModel { Success = true, Mail = null };
        }
    }
}