using System;
using System.IO;
using NUnit.Framework;

namespace Fanex.ESportModelingData.UnitTests
{
    [Category("Fanex.ESportModelingData.UnitTests.Setup")]
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}