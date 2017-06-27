using Fanex.Data.Repository;
using Fanex.Service.Core;
using NegativeBalance.Business.Services;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;
using SimpleInjector;

namespace NegativeBalance.IntegrationTests
{
    public static class NegativeBalanceInjector
    {
        public static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<LogService, LogServiceImp>();
            container.Register<IUMControlAdapter, UMControlAdapter>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<RepositoryBalanceService, RepositoryBalanceServiceImp>();
            container.Register<TransformService, TransformServiceImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailDeliverService, MailDeliverServiceImp>();
            container.Register<RecipientService, RecipientServiceImp>();
            container.Register<BalanceNotificationService, BalanceNotificationServiceImp>();
        }
    }
}