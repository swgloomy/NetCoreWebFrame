namespace Common
{
    public class ResultCode
    {

        /// <summary>
        /// 调用成功
        /// create by gloomy 2017-12-22 10:31:08
        /// </summary>
        public static string RESULT_SUCCESS = "00000";

        /// <summary>
        /// 未登陆
        /// create by gloomy 2017-12-22 10:31:19
        /// </summary>
        public static string NOT_LOGIN = "00001";

        /// <summary>
        /// 非法身份请求
        /// create by gloomy 2017-12-22 10:42:53
        /// </summary>
        public static string ILLEGAL_IDENTITY_REQUEST = "00002";

        /// <summary>
        /// 重复获取手机验证码
        /// create by gloomy 2017-12-25 16:21:52
        /// </summary>
        public static string PHONE_CODE_AGAIN = "00003";

        /// <summary>
        /// 手机号码错误
        /// create by gloomy 2017-12-25 16:30:03
        /// </summary>
        public static string PHONE_ERROR = "00004";

        /// <summary>
        /// 参数错误
        /// create by gloomy 2017-12-25 17:08:47
        /// </summary>
        public static string FORM_DATA_ERROR = "00005";

        /// <summary>
        /// 验证码错误
        /// create by gloomy 2017-12-25 17:51:53
        /// </summary>
        public static string PHONE_CODE_ERROR = "00006";

        /// <summary>
        /// 用户不存在
        /// create by gloomy 2017-12-26 10:58:06
        /// </summary>
        public static string USER_NOT_EXIST = "00007";

        /// <summary>
        /// 系统错误
        /// create by gloomy 2017-12-26 14:41:29
        /// </summary>
        public static string SYSTEM_ERROR = "00008";

        /// <summary>
        /// 城市编码错误
        /// create by gloomy 2017-12-26 16:50:11
        /// </summary>
        public static string CITY_CODE_FORMAT_ERROR = "00009";

        /// <summary>
        /// 用户状态编码错误
        /// </summary>
        public static string USER_STATUS_ERROR = "00010";

        /// <summary>
        /// 数据库运行失败
        /// </summary>
        public static string DB_RUN_ERROR = "00011";

        /// <summary>
        /// 上传文件不能为空
        /// </summary>
        public static string UP_LOAD_FILE_EMPTY = "00012";

        /// <summary>
        /// 用户存在
        /// create by gloomy 2017-12-26 10:58:06
        /// </summary>
        public static string USER_EXIST = "00013";
    }
}