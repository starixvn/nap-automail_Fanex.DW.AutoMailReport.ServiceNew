using System;
using Fanex.Service.Core;

namespace TopWinningAgent.Business.Services
{
    public class RepositoryServiceImp : RepositoryService
    {
        public bool IsDataAvailable()
        {
            return true;
        }

        public DateTime QueryLastDate()
        {
            return SystemTime.Now;
        }
    }
}