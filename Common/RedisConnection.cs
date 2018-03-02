using System;
using StackExchange.Redis;

namespace Common
{
    /// <summary>
    /// redis连接类
    /// create by gloomy 2017-12-27 14:00:18
    /// </summary>
    public abstract class RedisConnection : IDisposable
    {
        protected static ConnectionMultiplexer _redis;

        protected static ConnectionMultiplexer redis => _redis ?? (_redis = redisOpen());

        /// <summary>
        /// 获取数据库连接并打开
        /// </summary>
        /// <returns></returns>
        private static ConnectionMultiplexer redisOpen()
        {
            return ConnectionMultiplexer.Connect(ConfigHelp.ConfigObject["redisIp"].ToString());
        }

        public void Dispose()
        {
            _redis?.Dispose();
            _redis?.Close();
        }
    }
}