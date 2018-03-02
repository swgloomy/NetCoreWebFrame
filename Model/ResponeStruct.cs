using System;

namespace Model
{
    /// <summary>
    /// 项目出口对象
    /// create by gloomy 2017-12-21 18:13:52
    /// </summary>
    public class ResponeStruct
    {
        public ResponeStruct()
        {
            resultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,ms");
            resultData=new object();
            message = string.Empty;
            resultCode = string.Empty;
            totalCount = 0;
        }

        /// <summary>
        /// 返回状态码
        /// create by gloomy 2017-12-21 18:14:34
        /// </summary>
        public string resultCode { get; set; }

        /// <summary>
        /// 返回对象
        /// create by gloomy 2017-12-22 09:25:17
        /// </summary>
        public object resultData { get; set; }

        /// <summary>
        /// 返回消息
        /// create by gloomy 2017-12-22 09:25:51
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 返回时间
        /// create by gloomy 2017-12-22 10:37:37
        /// </summary>
        public string resultTime { get; set; }

        /// <summary>
        /// 总条数
        /// create by gloomy 2018-01-10 02:44:11
        /// </summary>
        public long totalCount { get; set; }
    }
}