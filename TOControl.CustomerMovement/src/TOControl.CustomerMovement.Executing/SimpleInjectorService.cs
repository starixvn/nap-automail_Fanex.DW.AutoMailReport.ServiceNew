using Fanex.Data.Repository;
using Fanex.Service.Core;
using SimpleInjector;
using TOControl.CustomerMovement.Business.Services;

namespace TOControl.CustomerMovement.Executing
{
    public static class SimpleInjectorService
    {
        public static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<LogService, LogServiceImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<DateService, DateServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<XMLService, XMLServiceImp>();
            container.Register<TOControlRepositoryService, TOControlRepositoryServiceImp>();
            container.Register<TransformDataService, TransformDataServiceImp>();

            container.Register<MailMovementService, MailMovementServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<NotificationMovementService, NotificationMovementServiceImp>();
        }
    }
}