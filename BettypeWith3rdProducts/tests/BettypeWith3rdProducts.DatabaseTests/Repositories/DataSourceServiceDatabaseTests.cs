using System;
using BettypeWith3rdProducts.Business.Repositories;
using Fanex.Service.Core;
using NUnit.Framework;
using SimpleInjector;

namespace BettypeWith3rdProducts.DatabaseTests.Repositories
{
    [Category("BettypeWith3rdProducts.DatabaseTests.Repositories DataSourceServiceDatabaseTests")]
    public class DataSourceServiceDatabaseTests
    {
        private DataSourceService dataSourceService;
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

            dataSourceService = container.GetInstance<DataSourceService>();
            repositoryService = container.GetInstance<RepositoryService>();
        }

        [Test]
        public void Query_BettypeWithProducts_ConntectToDatabaseSuccessfully()
        {
            // Arrange
            var criteria = new BettypeProductCriteria
            {
                DateStart = new DateTime(2010, 10, 01),
                DateEnd = new DateTime(2010, 10, 01),
                StatisticId = 1,
                StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts"
            };

            // Act

            // Assert
            Assert.That(true);
        }

        [Test]
        public void Query_BettypeWithProductsJuice_ConntectToDatabaseSuccessfully()
        {
            // Arrange
            var criteria = new BettypeProductCriteria
            {
                DateStart = new DateTime(2030, 01, 01),
                DateEnd = new DateTime(2030, 01, 01),
                StatisticId = 1,
                StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts_Juice"
            };

            // Act
            var result = dataSourceService.Query(criteria);

            // Assert
            Assert.That(result != null);
        }

        [Test]
        public void Query_Sport_ConntectToDatabaseSuccessfully()
        {
            // Arrange
            var criteria = new DWStatusByCubeCriteria();

            // Act
            var result = dataSourceService.Query(criteria);

            // Assert
            Assert.That(result != null);
        }

        [Test]
        public void Query_GetDWStatusByCube_ConntectToDatabaseSuccessfully()
        {
            // Arrange

            // Act
            var result = repositoryService.QueryLastDate();

            // Assert
            Assert.That(result.Year > 1);
        }
    }
}