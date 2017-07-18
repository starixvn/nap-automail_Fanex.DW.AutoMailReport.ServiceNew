using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service FileServiceUnitTests")]
    public class FileServiceUnitTests
    {
        [Test]
        public void ReadContentWhenDataIsAvailable_Always_Successfully()
        {
            // Arrange

            var fileService = new FileServiceImp();

            // Act

            // Assert
        }
    }
}
