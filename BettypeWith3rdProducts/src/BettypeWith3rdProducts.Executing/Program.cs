using System;
using BettypeWith3rdProducts.Business.Services;
using SimpleInjector;

namespace BettypeWith3rdProducts.Executing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                FanexDataRegisterService.ConfigureFanexData();

                Container container = new Container();
                SimpleInjectorService.Register(container);
                container.Verify(VerificationOption.VerifyOnly);

                var service = container.GetInstance<ExcelService>();
                service.Execute();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}