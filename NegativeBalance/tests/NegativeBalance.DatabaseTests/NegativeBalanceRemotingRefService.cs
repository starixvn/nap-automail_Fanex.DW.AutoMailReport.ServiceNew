using System;
using System.Configuration;
using Fanex.RemotingRef;

namespace NegativeBalance.DatabaseTests
{
    public static class NegativeBalanceRemotingRefService
    {
        public static void Register()
        {
            RefDataTypeContainer
                .ConnectTo(new Uri(ConfigurationManager.AppSettings["RemoteRefURL"]))
                .FromApplication("Fanex.NegativeBalance.Service")
                .RegisterSupportedLanguages("en-US", "zh-CN");
        }
    }
}