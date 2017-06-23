using System;
using System.Data.SqlClient;
using NUnit.Framework;

namespace Fanex.Service.Core.UnitTests
{
    [Category("Fanex.Service.Core.UnitTests LogServiceTests")]
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