using System;
using System.Net;
using System.Net.Sockets;

namespace Common
{
    /// <summary>
    /// 获取服务器IP地址
    /// create by gloomy 2018-01-30 17:45:19
    /// </summary>
    public class ServerIpUtil
    {
        /// <summary>
        /// 获取本机IP地址
        /// create by gloomy 2018-01-30 17:47:51
        /// </summary>
        /// <returns></returns>
        public string GetMyComputerIp()
        {
            var name = Dns.GetHostName();
            var ipadrlist = Dns.GetHostAddresses(name);
            foreach (var ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                    return ipa.ToString();
            }

            return string.Empty;
        }
    }
}