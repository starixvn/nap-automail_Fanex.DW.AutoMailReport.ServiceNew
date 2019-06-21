using System;
using Fanex.Data.Repository;
using Fanex.ESportModelingData.Business.Criterias;
using Fanex.ESportModelingData.Business.Models;
using Fanex.ESportModelingData.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.RepositoryServiceUnitTests")]
    public class RepositoryServiceUnitTests
    {
        [Test]
        public void Query_Always_MethodShouldBeCalled()
        {
            // Arrange
            var repository = Substitute.For<IDynamicRepository>();
            var dateStart = new DateTime(2019, 06, 17);
            var dateEnd = new DateTime(2019, 06, 23);

            // Act
            var service = new RepositoryServiceImp(repository);
            service.Query(dateEnd, dateEnd);

            // Assert
            repository.Received().Fetch<RawModelingDataModel>(Arg.Any<ESportModelingDataCriteria>());
        }
    }
}