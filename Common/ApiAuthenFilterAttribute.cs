using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Model;

namespace Common
{
    /// <summary>
    /// 请求拦截器
    /// create by gloomy 2017-12-20 17:05:49
    /// </summary>
    public class ApiAuthenFilterAttribute : ActionFilterAttribute
    {

        /// <summary>
        /// 入口拦截器
        /// create by gloomy 2017-12-20 17:06:40
        /// </summary>
        /// <param name="context">请求对象</param>
        private void AuthenExecuting(ActionExecutingContext context)
        {
            if (!NoFilterHelper.IssureFilter<NoAuthenFilterAttribute>(context))
            {
                var resultModel=new ResponeStruct();
                var controllerName = context.ActionDescriptor.RouteValues["controller"];
                var actionName = context.ActionDescriptor.RouteValues["action"];
                var userAuthen = context.HttpContext.Request.Cookies[ConfigHelp.ConfigObject["saveUserCookieName"].ToString()];
                var clientIp = HttpClientIp.GetMyClientIp(context.HttpContext.Request);

                if (userAuthen!=null)
                {
                    var decUserAuthen = new StringAesDes().Decrypt(userAuthen,ConfigHelp.ConfigObject["authenEncryptionKey"].ToString());
                    var userAuthenArray = decUserAuthen.Split(':');
                    if (userAuthenArray.Length == 2 && userAuthenArray[1]==clientIp)
                    {
                       //成功
                    }
                    resultModel.resultCode = ResultCode.ILLEGAL_IDENTITY_REQUEST;
                }
                else
                {
                    resultModel.resultCode = ResultCode.NOT_LOGIN;
                }
                context.HttpContext.Response.Redirect("/");

                HttpSendLog.ErrorLogAsync("ApiAuthenFilterAttribute AuthenExecuting controller:{0} action:{1} clientIp:{2} phone:{3} ",
                        controllerName, actionName, clientIp);

                //直接出去
                context.Result = AuthenResult(resultModel);
            }
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

        /// <summary>
        /// 出口拦截器
        /// create by gloomy 2017-12-20 17:08:06
        /// </summary>
        /// <param name="context">出口对象</param>
        private void AuthenExecuted(ActionExecutedContext context)
        {
            #region 暂时作废

//            var resultModel=new ResponeStruct();
//            var objectResultModel = context.Result as ObjectResult;
//            if (objectResultModel==null)
//            {
//                return;
//            }
//            resultModel.resultData = objectResultModel.Value;
//            context.Result = AuthenResult(resultModel);

            #endregion
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AuthenExecuting(context);
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            AuthenExecuted(context);
            base.OnActionExecuted(context);
        }
    }
}