using System.Collections.Generic;
using Fanex.Data.Repository;
using Fanex.UpdateExchangeRate.Business.Models;
using Fanex.UpdateExchangeRate.Business.Repositories;
using Fanex.UpdateExchangeRate.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace UpdateExchangeRate.UnitTests.Services
{
    [Category("UpdateExchangeRate.UnitTests.Services DataServiceUnitTests")]
    public class DataServiceUnitTests
    {
        [Test]
        public void Query_Always_MethodFetchShouldBeCalled()
        {
            // Arrange
            var dynamicRepository = Substitute.For<IDynamicRepository>();
            dynamicRepository.Fetch<CurrencyRate>(Arg.Any<Criteria>()).Returns(new List<CurrencyRate> { });

            // Act
            var service = new DataServiceImp(dynamicRepository);
            service.Query();

            // Assert
            dynamicRepository.Received().Fetch<CurrencyRate>(Arg.Any<Criteria>());
        }
    }
}