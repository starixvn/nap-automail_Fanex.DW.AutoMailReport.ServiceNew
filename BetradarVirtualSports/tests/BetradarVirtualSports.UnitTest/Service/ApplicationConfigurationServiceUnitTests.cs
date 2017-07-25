using System;
using System.IO;
using BetradarVirtualSportsDaily.Business.Models;
using BetradarVirtualSportsDaily.Business.Services;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service ApplicationConfigurationServiceUnitTests")]
    public class ApplicationConfigurationServiceUnitTests
    {
        [Test]
        public void Configurations_Always_ReturnRightContent()
        {
            // Arrange
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var expected = TestLoader.DataLoader<AppConfig>("AppConfig.json");

            // Act
            var service = new ApplicationConfigurationServiceImp();
            var result = service.Configurations;

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}