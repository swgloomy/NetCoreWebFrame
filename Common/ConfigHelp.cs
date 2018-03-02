using System.IO;
using Newtonsoft.Json.Linq;

namespace Common
{
    /// <summary>
    /// 配置文件帮助类
    /// create by gloomy 2017-12-21 15:26:47
    /// </summary>
    public class ConfigHelp
    {
        public static JObject ConfigObject = configProcess();

        /// <summary>
        /// 配置文件获取
        /// create by gloomy 2017-12-21 15:27:58
        /// </summary>
        /// <returns></returns>
        private static JObject configProcess()
        {
            return JObject.Parse(File.ReadAllText("./Configs/config.json"));
        }
    }
}