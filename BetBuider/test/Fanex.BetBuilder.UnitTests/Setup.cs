using NUnit.Framework;

namespace Fanex.BetBuilder.UnitTests
{
    [Category("Fanex.BetBuilder.UnitTests.Setup")]
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void Init()
        {
            RegisterService.RegisterServices();
        }
    }
}