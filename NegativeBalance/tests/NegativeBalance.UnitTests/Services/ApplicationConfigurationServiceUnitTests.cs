using System;
using System.IO;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Services;
using Newtonsoft.Json;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services ApplicationConfigurationServiceUnitTests")]
    public class ApplicationConfigurationServiceUnitTests
    {
        [Test]
        public void Configurations_Always_ReturnRightContent()
        {
            // Arrange
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var expected = CreateAppConfig();

            // Act
            var service = new ApplicationConfigurationServiceImp();
            var result = service.Configurations;
            var json = JsonConvert.SerializeObject(expected, Formatting.Indented);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private AppConfig CreateAppConfig()
        {
            return new AppConfig
            {
                IntervalInSecond = 15,
                IsTest = true,
                IsSendMailFromConfig = true,
                MailTo = "robert.tran@starixsoft.com",
                MailCc = string.Empty,
                MailBcc = string.Empty,
                Subject = "Automail Report - {0} - List of Members with Negative Balance By Match - {1}",
                Title = "Auto Generated Report Administrator",                
            };
        }
    }
}