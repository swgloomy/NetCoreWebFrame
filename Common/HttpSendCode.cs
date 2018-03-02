using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 发送短信验证码
    /// create by gloomy 2017-12-25 16:47:47
    /// </summary>
    public class HttpSendCode
    {
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="phoneCode">验证码</param>
        /// <returns></returns>
        public async Task HttpPostPhoneCodeAsync(string clientIp,string phone, string phoneCode)
        {
            await Task.Run(() =>
            {
                try
                {
                    var paramsList = new Dictionary<string, string>();
                    paramsList.Add("mob", phone);
                    paramsList.Add("msg", string.Format(ConfigHelp.ConfigObject["phoneCodeMessage"].ToString(),phoneCode));
                    paramsList.Add("pswd", ConfigHelp.ConfigObject["messagePostPswd"].ToString());
                    var headList = new Dictionary<string, string>();
                    headList.Add("clientIPAddr", clientIp);
                    headList.Add("requestAccount", ConfigHelp.ConfigObject["messagePostAccount"].ToString());
                    HttpHelper.GetResponseString(HttpHelper.CreatePostHttpResponse(
                        ConfigHelp.ConfigObject["messagePostUrlPath"].ToString(), paramsList, headList, 300,
                        string.Empty,
                        null));
                    HttpSendLog.InfoLogAsync(
                        "HttpSendCode HttpPostPhoneCodeAsync run success! clientIp:{0} phone:{1} phoneCode:{2} ",
                        clientIp, phone, phoneCode);
                }
                catch (Exception e)
                {
                    HttpSendLog.ErrorLogAsync(
                        "HttpSendCode HttpPostPhoneCodeAsync run success! clientIp:{0} phone:{1} phoneCode:{2} err:{3}",
                        clientIp, phone, phoneCode, e);
                }
            });
        }
    }
}