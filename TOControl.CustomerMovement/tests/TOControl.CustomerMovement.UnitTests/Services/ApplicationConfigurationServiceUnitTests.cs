using System;
using System.IO;
using NUnit.CompareNetObjects;
using NUnit.Framework;
using TOControl.CustomerMovement.Business.Models;
using TOControl.CustomerMovement.Business.Services;

namespace TOControl.CustomerMovement.UnitTests.Services
{
    [Category("TOControl.CustomerMovement.UnitTests.Services ApplicationConfigurationServiceUnitTests")]
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
                Subject = "Noticeable events report",
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