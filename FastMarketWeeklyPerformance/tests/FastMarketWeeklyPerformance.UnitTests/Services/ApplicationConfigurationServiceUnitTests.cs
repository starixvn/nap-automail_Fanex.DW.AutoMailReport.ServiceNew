using System;
using System.IO;
using FastMarketWeeklyPerformance.Business.Models;
using FastMarketWeeklyPerformance.Business.Services;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace FastMarketWeeklyPerformance.UnitTests.Services
{
    [Category("FastMarketWeeklyPerformance.UnitTests.Services ApplicationConfigurationServiceUnitTests")]
    public class ApplicationConfigurationServiceUnitTests
    {
        [Test]
        public void AppConfig_Always_ReturnRightContent()
        {
            // Arrange
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var expected = CreateAppConfig();

            // Act
            var service = new ApplicationConfigurationServiceImp();
            var result = service.Configurations;

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private AppConfig CreateAppConfig()
        {
            return new AppConfig
            {
                ExportDataDirectory = @"\\10.70.70.242\www\Olympus\Fanex.Excel\FastMarketWeeklyPerformance",
                ExcelPasswordProtected = string.Empty,
                ReportDateRange = ReportDateRange.Weekly,
                IsCheckEnoughData = true,
                FunctionIdNodata = 90,
                Subject = "Automail - Fast Market - Weekly Performance",
                Title = "Auto Generated Report Administrator"
            };
        }
    }
}