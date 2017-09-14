using System;
using System.IO;
using MaxLossControl.CustomerMovement.Business.Models;
using MaxLossControl.CustomerMovement.Business.Services;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace MaxLossControl.CustomerMovement.UnitTests.Services
{
    [Category("MaxLossControl.CustomerMovement.UnitTests.Services ApplicationConfigurationServiceUnitTests")]
    public class ApplicationConfigurationServiceUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void Configurations_Always_ReturnRightConfig()
        {
            // Arrange
            var expected = new AppConfig
            {
                PeriodDate = 2,
                Subject = "Loss Monitor:  Noticeable events report",
                Title = "Auto Generated Report Administrator"
            };

            // Act
            var service = new ApplicationConfigurationServiceImp();
            var result = service.Configurations;

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}