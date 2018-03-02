using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Common
{
    /// <summary>
    /// redis帮助类
    /// create by gloomy 2017-12-21 15:51:22
    /// </summary>
    public class RedisProcess : RedisConnection
    {
        /// <summary>
        /// 设置redis存储值
        /// </summary>
        /// <param name="saveKey">存储的键</param>
        /// <param name="saveValue">存储的值</param>
        /// <param name="saveTimeLong">存储时长(单位秒)</param>
        public static async Task SetRedis(string saveKey, string saveValue,int saveTimeLong)
        {
            await Task.Run(() =>
            {
                try
                {
                    var timeNow = DateTime.Now;
                    var db = redis.GetDatabase(Convert.ToInt32(ConfigHelp.ConfigObject["redisDatabase"].ToString()));
                    db.StringSet(saveKey, saveValue,timeNow.AddSeconds(saveTimeLong)-timeNow);
                }
                catch (Exception e)
                {
                    HttpSendLog.ErrorLogAsync("RedisProcess SetRedis run err! saveKey:{0} saveValue:{1} err: {2}",
                        saveKey,
                        saveValue, e);
                }
            });
        }

        public static void SetRedisNoAsync(string saveKey, string saveValue,int saveTimeLong)
        {
                try
                {
                    var timeNow = DateTime.Now;
                    var db = redis.GetDatabase(Convert.ToInt32(ConfigHelp.ConfigObject["redisDatabase"].ToString()));
                    db.StringSet(saveKey, saveValue,timeNow.AddSeconds(saveTimeLong)-timeNow);
                }
                catch (Exception e)
                {
                    HttpSendLog.ErrorLogAsync("RedisProcess SetRedis run err! saveKey:{0} saveValue:{1} err: {2}",
                        saveKey,
                        saveValue, e);
                }
        }

        /// <summary>
        /// 获取redis存储值
        /// </summary>
        /// <param name="saveKey">存储的键</param>
        /// <returns></returns>
        public static string GetRedis(string saveKey)
        {
            var redisSaveValue = string.Empty;
            try
            {
                var db = redis.GetDatabase(Convert.ToInt32(ConfigHelp.ConfigObject["redisDatabase"].ToString()));
                redisSaveValue = db.StringGet(saveKey);
            }
            catch (Exception e)
            {
                HttpSendLog.ErrorLogAsync("RedisProcess GetRedis run err! saveKey:{0} err: {2}", saveKey, e);
            }

            return redisSaveValue??string.Empty;
        }
    }
}