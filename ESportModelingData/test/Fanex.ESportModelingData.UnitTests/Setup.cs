using System;
using System.IO;
using Fanex.ESportModelingData.Business.Models;
using Fanex.ESportModelingData.Business.Services;
using Newtonsoft.Json;
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
            var text = File.ReadAllText("Templates/ESportModelingDataSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }
    }
}