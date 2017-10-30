using System;
using NSubstitute;
using NUnit.Framework;
using TOControl.Notification.Business.Models;
using TOControl.Notification.Business.Services;

namespace TOControl.Notification.Business.UnitTests.Services
{
    [Category("TOControl.Notification.Business.UnitTests.Services ")]
    public class TransformDataServiceUnitTests
    {
        private TOControlRepositoryService repositoryService;

        [SetUp]
        public void Initialize()
        {
            repositoryService = Substitute.For<TOControlRepositoryService>();
        }

        [Test]
        public void TransformGroupHeadline_Always_ReturnRightData()
        {
            // Arrange
            var rawData = TestLoader.DataLoader<RawData>("RawData-GroupHeadline.json");
            var groupHeadlineRaw = new GroupHeadlineRaw { CustId = 1, OldCustId = 2 };
            repositoryService.QueryCustomerByCustId(1).Returns(new CustomerInfomation { Username = "SUPERTESTL3103" });
            repositoryService.QueryCustomerByCustId(2).Returns(new CustomerInfomation { Username = "CTH" });
            var expected = new GroupHeadline { Date = new DateTime(2017, 03, 10, 4, 40, 21), Id = 8, NewHeadline = "SUPERTESTL3103", OldHeadline = "CTH" };

            // Act
            var service = new TransformDataServiceImp(repositoryService);
            var result = service.TransformGroupHeadline(groupHeadlineRaw, rawData);

            // Assert
        }
    }
}