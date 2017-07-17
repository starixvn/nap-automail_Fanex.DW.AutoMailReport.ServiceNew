using System;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BetradarVirtualSports.UnitTest.Service
{
    [Category("BetradarVirtualSports.UnitTest.Service DistributedNotificationUnitTests")]
    public class MailServiceUnitTests
    {
        [Test]
        public void Execute_Always_Successfully()
        {
            // Arrange
            SystemTime.Set(new DateTime(2017, 01, 01));
            var expected = new Mail
            {
                Subject = $"[Betradar Virtual Sports] - Daily Report {SystemTime.Now:mm/dd/yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = TestLoader.DataLoader("EmailTemplate-expected.txt")
            };

            // Act
            var service = new MailServiceImp();
            var result = service.Execute();

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }
    }
}