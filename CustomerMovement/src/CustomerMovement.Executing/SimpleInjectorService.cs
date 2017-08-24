using CustomerMovement.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using SimpleInjector;

namespace CustomerMovement.Executing
{
    public static class SimpleInjectorService
    {
        public static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<LogService, LogServiceImp>();
            container.Register<DataService, FakeDataService>();
            container.Register<DateService, DateServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailMovementService, MailMovementServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<NotificationMovementService, NotificationMovementServiceImp>();
        }
    }
}