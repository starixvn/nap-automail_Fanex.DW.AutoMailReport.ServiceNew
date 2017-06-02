using System.Linq;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Repositories;
using NUnit.Framework;
using SimpleInjector;

namespace BettypeWith3rdProducts.DatabaseTests.Repositories
{
    [Category("BettypeWith3rdProducts.DatabaseTests.Repositories DataSourceServiceDatabaseTests")]
    public class DataSourceServiceDatabaseTests
    {
        private DataSourceService service;

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

            service = container.GetInstance<DataSourceService>();
        }

        [Test]
        public void QuerySports_RealWiLoss_ConnectToDatabaseSuccessfully()
        {
            // Act
            var result = service.QuerySports(DataType.RealWiLoss);

            // Assert
            Assert.That(result.Count() > 0);
        }

        [Test]
        public void QuerySports_VirtualWinLoss_ConnectToDatabaseSuccessfully()
        {
            // Act
            var result = service.QuerySports(DataType.VirtualWinLoss);

            // Assert
            Assert.That(result.Count() > 0);
        }

        [Test]
        public void QueryDataSouce_Always_ConnectToDatabaseSuccessfully()
        {
            // Arrange
            var sourceMapping = new SourceMapping { DataType = DataType.RealWiLoss, SheetName = "Real", StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts" };

            // Act
            var result = service.QueryDataSouce(sourceMapping, DateRange.Manual, ViewOfData.All, 1);

            // Assert
            Assert.That(result.Columns.Count > 0);
        }
    }
}