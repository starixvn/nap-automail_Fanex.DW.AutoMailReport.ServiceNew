using System;
using System.Collections.Generic;
using Fanex.Data.Repository;
using MaxLossControl.CustomerMovement.Business.Models;
using MaxLossControl.CustomerMovement.Business.Repositories;
using MaxLossControl.CustomerMovement.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace MaxLossControl.CustomerMovement.UnitTests.Services
{
    [Category("MaxLossControl.CustomerMovement.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        [Test]
        public void Query_Always_ConntectToDatabaseSuccessfully()
        {
            // Arrange
            var dateService = Substitute.For<DateService>();
            var dynamicRepository = Substitute.For<IDynamicRepository>();
            dynamicRepository.Fetch<Movement>(Arg.Any<Criteria>()).Returns(new List<Movement> { new Movement { GroupHeadline = "Supertest3", GroupMember = "Supertest4", Action = "Add", Date = new DateTime(2017, 07, 01) } });

            // Act
            var service = new DataServiceImp(dateService, dynamicRepository);
            service.Query();

            // Assert
            dynamicRepository.Received().Fetch<GroupHeadline>(Arg.Any<Criteria>());
            dynamicRepository.Received().Fetch<Movement>(Arg.Any<Criteria>());
            dynamicRepository.Received().Fetch<DirectCustomer>(Arg.Any<Criteria>());
        }
    }
}