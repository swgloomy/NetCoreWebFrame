using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 项目日志类
    /// create by gloomy 2017-12-25 15:26:14
    /// </summary>
    public class HttpSendLog
    {
        /// <summary>
        /// 发送日志
        /// create by gloomy 2017-12-25 15:46:08
        /// </summary>
        /// <param name="logState">日志状态(INFO,ERROR,DEBUG)</param>
        /// <param name="logContent"></param>
        /// <returns></returns>
        private static void SendLog(string logState, string logContent)
        {
            if (!Convert.ToBoolean(ConfigHelp.ConfigObject["isWriteLog"].ToString()))
            {//不打印日志
                Console.WriteLine("{0}: {1}",logState,logContent);
                return;
            }
                var dicArray = new Dictionary<string, string>();
                dicArray.Add("logState", logState);
                dicArray.Add("logContent", logContent);
            HttpHelper.GetResponseString(HttpHelper.CreatePostHttpResponse(
                string.Format("http://127.0.0.1:{0}{1}/unitWriteLog",ConfigHelp.ConfigObject["serverListeningPort"] ,ConfigHelp.ConfigObject["rootPrefix"])
                , dicArray, new Dictionary<string, string>(), 3000,
                string.Empty, null));
        }



        /// <summary>
        /// 发送正常记录日志
        /// create by gloomy 2017-12-26 11:01:24
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static async Task InfoLogAsync(string format, params object[] args)
        {
            await Task.Run(() =>
            {
                SendLog("info", string.Format(format, args));
            });
        }

        /// <summary>
        /// 发送错误记录日志
        /// create by gloomy 2017-12-26 11:01:24
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static async Task ErrorLogAsync(string format, params object[] args)
        {
            await Task.Run(() =>
            {
                SendLog("error", string.Format(format, args));
            });
        }

        /// <summary>
        /// 发送错误记录日志
        /// create by gloomy 2017-12-26 11:01:24
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static async Task DebugLogAsync(string format, params object[] args)
        {
            await Task.Run(() =>
            {
                SendLog("debug", string.Format(format, args));
            });
        }
    }
}