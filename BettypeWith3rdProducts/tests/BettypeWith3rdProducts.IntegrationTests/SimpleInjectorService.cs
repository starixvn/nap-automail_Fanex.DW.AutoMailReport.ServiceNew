using BettypeWith3rdProducts.Business.Repositories;
using BettypeWith3rdProducts.Business.Services;
using Fanex.Data.Repository;
using SimpleInjector;

namespace BettypeWith3rdProducts.IntegrationTests
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<DateRangeServiceProvider, DateRangeServiceProviderImp>();
            container.Register<ParameterService, ParameterServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();            
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<SportSplitterService, SportSplitterServiceImp>();
            container.Register<DataService, DataServiceImp>();
        }
    }
}