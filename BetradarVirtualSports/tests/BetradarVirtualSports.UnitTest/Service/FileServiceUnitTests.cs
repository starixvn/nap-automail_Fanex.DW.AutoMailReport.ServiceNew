using System;
using System.IO;
using BetradarVirtualSports.Business.Services;
using Fanex.Service.Core;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service FileServiceUnitTests")]
    public class FileServiceUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void ReadContentWhenDataIsAvailable_Always_Successfully()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            var expected = TestLoader.DataLoader("ReadContentWhenDataAvailable-expected.html");

            // Act
            var fileService = new FileServiceImp();
            var result = fileService.ReadContentWhenDataIsAvailable();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}