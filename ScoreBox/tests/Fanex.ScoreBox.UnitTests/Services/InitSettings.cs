using Fanex.ScoreBox.Business.Models;
using Fanex.ScoreBox.Business.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

namespace Fanex.ScoreBox.UnitTests.Services
{
    [SetUpFixture]
    public class InitSettings
    {
        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText("Templates/DailyScoreBoxSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }
    }
}