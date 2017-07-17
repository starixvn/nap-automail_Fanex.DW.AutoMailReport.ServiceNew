using System;
using System.Data.SqlClient;
using Fanex.Service.Core;
using NUnit.Framework;

namespace BettypeWith3rdProducts.IntegrationTests.Services
{
    [Category("BettypeWith3rdProducts.IntegrationTests.Services LogServiceTests")]
    public class LogServiceTests
    {
        [Test]
        public void Log_Always_WriteLogSuccessfully()
        {
            // Arrange
            var exception = new Exception("Testing");

            // Act
            var service = new LogServiceImp();
            service.Log(exception);
        }

        [Test]
        public void Log_SqlException_WriteLogSuccessfully()
        {
            var service = new LogServiceImp();
            try
            {
                var connection = new SqlConnection("server=local");
                connection.Open();
            }
            catch (SqlException sqlException)
            {
                service.Log(sqlException);
            }
        }
    }
}