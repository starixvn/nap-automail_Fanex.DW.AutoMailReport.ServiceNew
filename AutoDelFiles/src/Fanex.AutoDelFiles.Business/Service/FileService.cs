using Fanex.AutoDelFiles.Business.Models;

namespace Fanex.AutoDelFiles.Business.Service
{
    public interface FileService
    {
        void Execute(PathModel path);
    }
}