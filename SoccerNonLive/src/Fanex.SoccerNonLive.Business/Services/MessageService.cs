using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using System.Collections.Generic;

namespace Fanex.SoccerNonLive.Business.Services
{
    public interface MessageService
    {
        LetstalkModel CreateLetstalkModel(string fileName, string letstalkGroup);
    }

    public class MessageServiceImp : MessageService
    {
        public LetstalkModel CreateLetstalkModel(string fileName, string letstalkGroup)
        {
            return null;
        }
    }
}