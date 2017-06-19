using System;
using NUnit.Framework;

namespace Fanex.Service.Core.UnitTests
{
    [Category("Fanex.Service.Core.UnitTests SystemTimeUnitTests")]
    public class SystemTimeUnitTests
    {
        [Test]
        public void Set_Always_Succussfully()
        {
            // Arrange
            var date = new DateTime(2017, 01, 01, 01, 01, 01);

            // Act
            SystemTime.Set(date);
            var result = SystemTime.Now;
            SystemTime.Reset();

            // Assert
            Assert.That(result, Is.EqualTo(date));
        }
    }
}