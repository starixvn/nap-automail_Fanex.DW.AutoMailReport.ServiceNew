using System.Collections.Generic;

namespace Fanex.DHWL.Business.Models
{
    public class RocketChatModel
    {
        public string RocketChatUrl { get; set; }

        public Dictionary<string, ChannelModel> Channels = new Dictionary<string, ChannelModel>();

        public Dictionary<string, ChannelModel> NewChartChannels = new Dictionary<string, ChannelModel>();
    }
}