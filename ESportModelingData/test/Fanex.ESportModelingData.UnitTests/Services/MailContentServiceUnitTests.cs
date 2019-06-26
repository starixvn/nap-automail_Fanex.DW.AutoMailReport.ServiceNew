using System.Collections.Generic;
using System.IO;
using Fanex.ESportModelingData.Business.Services;
using NUnit.Framework;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.MailContentServiceUnitTests")]
    public class MailContentServiceUnitTests
    {
        [Test]
        public void GenerateMailContent_Always_ReturnRightContent()
        {
            // Arrange
            var fileName = new List<string> { "FileName" };
            var expected = File.ReadAllText("Templates/EmailContent.txt");

            // Act
            var service = new MailContentServiceImp();
            var result = service.GenerateMailContent(fileName);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}