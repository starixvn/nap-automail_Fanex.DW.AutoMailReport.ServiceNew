using BettypeWith3rdProducts.Business.Services;
using NUnit.Framework;
using SimpleInjector;

namespace BettypeWith3rdProducts.DatabaseTests.Repositories
{
    [Category("BettypeWith3rdProducts.DatabaseTests.Repositories DataSourceServiceDatabaseTests")]
    public class DataSourceServiceDatabaseTests
    {
        private RepositoryService repositoryService;

        [OneTimeSetUp]
        public void InitDataBase()
        {
            FanexDataRegisterService.ConfigureFanexData();
            Initialize();
        }

        private void Initialize()
        {
            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            repositoryService = container.GetInstance<RepositoryService>();
        }
    }
}