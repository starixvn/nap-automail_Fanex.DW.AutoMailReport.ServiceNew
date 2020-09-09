using System.IO;

namespace Fanex.Letstalk.OneTime.Business.Models
{
    public interface FileService
    {
        string LoadMessage(bool isMail);
    }

    public class FileServiceImp : FileService
    {
        public string LoadMessage(bool isMail)
        {
            if (isMail)
            {
                return File.ReadAllText("Templates/EmailTemplate.cshtml");
            }
            else
            {
                return File.ReadAllText("Templates/Message.txt");
            }
        }
    }
}