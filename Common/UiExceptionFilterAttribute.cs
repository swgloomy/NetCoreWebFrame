using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Model;
using Newtonsoft.Json;

namespace Common
{
    /// <summary>
    /// 错误拦截器
    /// create by gloomy 2017-12-25 15:02:19
    /// </summary>
    public class UiExceptionFilterAttribute:ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            Console.WriteLine(context.Exception.ToString());
            if (!Convert.ToBoolean(ConfigHelp.ConfigObject["isWriteLog"].ToString()))
            {
                context.ExceptionHandled = true; // mark exception as handled
            }

            HttpSendLog.ErrorLogAsync(
                "UiExceptionFilterAttribute OnException! httpReuqstUrl:{0} method:{1} {2} {3} err:{4}",
                context.HttpContext.Request.GetDisplayUrl(), context.HttpContext.Request.Method,
                context.HttpContext.Request.Form.Aggregate("requestForm:",
                    (current, item) => string.Format("{0} {1}:{2}", current, item.Key, item.Value)),
                context.RouteData.Values.Aggregate("RouteData:",
                    (current, item) => string.Format("{0} {1}:{2}", current, item.Key, item.Value)),
                context.Exception.ToString());



            context.Result = AuthenResult(new ResponeStruct
            {
                resultCode = ResultCode.SYSTEM_ERROR
            });
        }

        /// <summary>
        /// 拦截器返回出口
        /// </summary>
        /// <param name="resultJsonStr">返回对象json字符串</param>
        /// <returns></returns>
        private ContentResult AuthenResult(ResponeStruct resultModel)
        {
            return new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(resultModel)
            };
        }

    }
}