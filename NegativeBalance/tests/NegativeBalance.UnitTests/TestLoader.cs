using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NegativeBalance.UnitTests
{
    public static class TestLoader
    {
        public static T DataLoader<T>(string fileName)
        {
            var fullName = $@"{TestContext.CurrentContext.TestDirectory}\Templates\{fileName}";
            var text = File.ReadAllText(fullName);
            var data = JsonConvert.DeserializeObject<T>(text);
            return data;
        }

        public static string DataLoader(string fileName)
        {
            var fullName = $@"{TestContext.CurrentContext.TestDirectory}\Templates\{fileName}";
            return File.ReadAllText(fullName);
        }
    }
}