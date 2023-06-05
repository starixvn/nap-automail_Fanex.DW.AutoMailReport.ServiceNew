using EnterpriseDT.Net.Ftp;

namespace Fanex.AutoSyncFiles.Business.Services
{
    public class Test
    {
        public static void DownloadAsync()
        {
            try
            {
                var ftp = new FTPConnection();
                ftp.ServerAddress = "10.18.200.131";
                ftp.UserName = "olympus";
                ftp.Password = "Pro_olympus_@1234aa";
                ftp.ServerPort = 2121;

                ftp.Connect();
                ftp.DownloadFile("S:/Browsers", "/nap/mail-web-api-test/excel/LOCAL/2021/07", WriteMode.Overwrite);
                ftp.Close();
            }
            catch
            {
            }
        }
    }
}