using Microsoft.AspNetCore.Http;

namespace Common
{
    /// <summary>
    /// 客户端IP操作类
    /// create by gloomy 2017-12-25 16:04:22
    /// </summary>
    public class HttpClientIp
    {
        /// <summary>
        /// 获取客户端IP地址
        /// create by gloomy 2017-12-25 16:06:15
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static string GetMyClientIp(HttpRequest request)
        {
            var clientIp = request.Headers["X-Forwarded-For"].ToString();
            var index = clientIp.IndexOf(",");
            if (index>=0)
            {
                clientIp = clientIp.Substring(0, index);
            }
            if (string.IsNullOrWhiteSpace(clientIp))
            {
                clientIp=request.Headers["X-Real-Ip"].ToString();
                if (string.IsNullOrWhiteSpace(clientIp))
                {
                    clientIp=request.Headers["X-Appengine-Remote-Addr"].ToString();
                    if (string.IsNullOrWhiteSpace(clientIp))
                    {
                        clientIp = request.Host.Host;
                    }
                }
            }

            return clientIp;
        }
    }
}