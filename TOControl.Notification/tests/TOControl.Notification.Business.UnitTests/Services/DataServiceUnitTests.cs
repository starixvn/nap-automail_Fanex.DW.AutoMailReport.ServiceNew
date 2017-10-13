using System;
using System.Collections.Generic;
using Fanex.Data.Repository;
using NSubstitute;
using NUnit.Framework;
using TOControl.Notification.Business.Models;
using TOControl.Notification.Business.Services;

namespace TOControl.Notification.Business.UnitTests.Services
{
    [Category("TOControl.Notification.Business.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        private DateService dateService;
        private XMLService xmlService;
        private TransformDataService transformDataService;
        private TOControlRepositoryService repositoryService;

        [SetUp]
        public void Initialize()
        {
            dateService = Substitute.For<DateService>();
            xmlService = new XMLServiceImp();
            transformDataService = Substitute.For<TransformDataService>();
            repositoryService = Substitute.For<TOControlRepositoryService>();

            dateService.FromDate.Returns(new DateTime(2017, 01, 01));
            dateService.ToDate.Returns(new DateTime(2017, 01, 02));
        }

        [Test]
        public void Query_Always_ReturnRightData()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}