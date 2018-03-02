using Common;

namespace DAL
{
    /// <summary>
    /// 数据库连接
    /// create by gloomy 2017-12-25 17:56:49
    /// </summary>
    public abstract class DBConnection
    {
        protected string sqlConnectionStr=string.Format("server={0};port={1};database={2};user id={3};password={4};SslMode=none",
            ConfigHelp.ConfigObject["dbHost"], ConfigHelp.ConfigObject["dbPort"],
            ConfigHelp.ConfigObject["dbName"], ConfigHelp.ConfigObject["dbUser"],
            ConfigHelp.ConfigObject["dbPassword"]);
    }
}