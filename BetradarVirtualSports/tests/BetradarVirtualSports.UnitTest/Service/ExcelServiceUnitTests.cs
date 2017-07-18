using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetradarVirtualSports.Business.Services;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service ExcelServiceUnitTests")]
    public class ExcelServiceUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void Export_Always_Successfully()
        {
            // Arrange

            // Act
            var service = new ExcelServiceImp();
            service.Export().Save();

            // Assert
        }
    }
}
