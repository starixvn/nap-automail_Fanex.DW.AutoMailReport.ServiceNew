using System;
using System.Collections.Generic;
using System.IO;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using NUnit.CompareNetObjects;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services ApplicationConfigurationServiceUnitTests")]
    public class ApplicationConfigurationServiceUnitTests
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void SourceMappings_Always_ReturnRightContent()
        {
            // Arrange
            Initialize();
            var expected = CreateSourceMappings();

            // Act
            var service = new ApplicationConfigurationServiceImp();
            var result = service.SourceMappings;

            // Assert
            Assert.That(result, IsDeeplyEqual.To(expected));
        }

        private List<SourceMapping> CreateSourceMappings()
        {
            return new List<SourceMapping>
            {
                new SourceMapping { DataType = DataType.RealWiLoss, SheetName = "Real", StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts" },
                new SourceMapping { DataType = DataType.VirtualWinLoss, SheetName = "Virtual", StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts" },
                new SourceMapping { DataType = DataType.RealWiLoss, SheetName = "Real Juice & Leftover", StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts_Juice" },
                new SourceMapping { DataType = DataType.VirtualWinLoss, SheetName = "Virtual Juice & Leftover", StoreProcedure = "Acc_Rpt_Automail_BettypeWithProducts_Juice" }
            };
        }     

        [Test]
        public void FunctionIdNodata_Always_Return90()
        {
            // Arrange
            var expected = 90;

            // Act
            var service = new ApplicationConfigurationServiceImp();
            var result = service.FunctionIdNodata;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void IsCheckEnoughData_Always_ReturnTrue()
        {
            // Arrange
            var expected = true;

            // Act
            var service = new ApplicationConfigurationServiceImp();
            var result = service.IsCheckEnoughData;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}