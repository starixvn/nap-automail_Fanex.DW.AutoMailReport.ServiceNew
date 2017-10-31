using System;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using NUnit.Framework;

namespace Fanex.Service.Core.DatabaseTests
{
    [Category("Fanex.Service.Core.DatabaseTests DataSourceServiceDatabaseTests")]
    public class DataSourceServiceDatabaseTests
    {
        [OneTimeSetUp]
        public void ConfigureFanexData()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider($"~/App_Data/Local", false, true)
                .Run();
        }

        [Test]
        public void Query_Always_ConnectToDatabaseSuccessfully()
        {
            // Arrange
            var service = new DataSourceServiceImp();

            // Act
            var result = service.Query(new TestCriteria());

            // Assert
            Assert.That(result.Rows.Count > 0);
        }

        [Test]
        public void QueryMultiple_Always_ConnectToDatabaseSuccessfully()
        {
            // Arrange
            var service = new DataSourceServiceImp();

            // Act
            var result = service.QueryMultiple(new TestCriteria());

            // Assert
            Assert.That(result.Tables.Count > 0);
        }
    }
}