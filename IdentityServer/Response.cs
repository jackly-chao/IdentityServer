using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    /// <summary>
    /// 响应
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        public Response(int code = 200, bool success = true, string msg = "请求成功")
        {
            Code = code;
            switch (Code)
            {
                case StatusCodes.Status401Unauthorized:
                    Message = "很抱歉,您无权访问该接口,请确保已经登录!";
                    Success = false;
                    break;
                case StatusCodes.Status403Forbidden:
                    Message = "很抱歉,您的访问权限不够,请联系管理员!";
                    Success = false;
                    break;
                default:
                    Message = msg;
                    Success = success;
                    break;
            }
        }
    }
}
