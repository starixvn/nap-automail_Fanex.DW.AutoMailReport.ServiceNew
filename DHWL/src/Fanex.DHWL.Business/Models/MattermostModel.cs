using System.Collections.Generic;

namespace Fanex.DHWL.Business.Models
{
    public class MattermostModel
    {
        public string MattermostUrl { get; set; }

        public Dictionary<string, ChannelModel> Channels = new Dictionary<string, ChannelModel>();
    }
}