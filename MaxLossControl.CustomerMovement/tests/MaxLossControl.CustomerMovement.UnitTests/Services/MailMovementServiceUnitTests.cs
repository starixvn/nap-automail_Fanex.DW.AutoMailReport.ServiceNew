using System;
using System.Collections.Generic;
using System.IO;
using Fanex.MailDelivery.Core.Models;
using MaxLossControl.CustomerMovement.Business.Models;
using MaxLossControl.CustomerMovement.Business.Services;
using NSubstitute;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace MaxLossControl.CustomerMovement.UnitTests.Services
{
    [Category("MaxLossControl.CustomerMovement.UnitTests.Services MailMovementServiceUnitTests")]
    public class MailMovementServiceUnitTests
    {
        private FileService fileService;
        private AppConfig appConfig;
        private DateService dateService;
        private ApplicationConfigurationService appConfigService;

        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            fileService = Substitute.For<FileService>();
            appConfigService = Substitute.For<ApplicationConfigurationService>();
            appConfig = new AppConfig
            {
                PeriodDate = 2,
                Subject = "Noticeable events report",
                Title = "Auto Generated Report Administrator"
            };

            appConfigService.Configurations.Returns(appConfig);
        }

        [Test]
        public void Execute_Always_ReturnRightContentMail()
        {
            // Arrange
            var data = new Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>>(new List<GroupHeadline>(), new List<Movement>(), new List<DirectCustomer>());
            var fromDate = new DateTime(2017, 08, 29);
            var toDate = new DateTime(2017, 08, 31);
            dateService = Substitute.For<DateService>();
            dateService.FromDate.Returns(fromDate);
            dateService.ToDate.Returns(toDate);
            fileService.ReadContent(Arg.Any<Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>>>(), Arg.Any<DateService>())
                .Returns(File.ReadAllText(@"Templates\FileService-ReadContent-Expected.txt"));

            var expected = new Mail
            {
                Body = File.ReadAllText(@"Templates\FileService-ReadContent-Expected.txt"),
                Subject = "Noticeable events report from 08/29/2017 to 08/31/2017",
                Title = "Auto Generated Report Administrator"
            };

            // Act
            var service = new MailMovementServiceImp(fileService, appConfigService, dateService);
            var result = service.Execute(data);

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}