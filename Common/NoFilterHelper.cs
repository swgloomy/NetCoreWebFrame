﻿
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common
{
    public class NoFilterHelper
    {
        /// <summary>
        /// 非拦截器判断
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IssureFilter<T>(FilterContext context)
        {
            foreach (var filterDescriptors in context.ActionDescriptor.FilterDescriptors)
            {
                if (filterDescriptors.Filter.GetType() == typeof(T))
                {
                    return true;
                }
            }
            return false;
        }
    }
}