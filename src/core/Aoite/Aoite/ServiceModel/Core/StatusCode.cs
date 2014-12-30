using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 定义响应定义的状态代码的值。
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// 表示响应成功。
        /// </summary>
        OK = 200,
        /// <summary>
        /// 表示请求无效。
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// 表示请求未经授权。
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// 表示找不到契约。
        /// </summary>
        NotFound = 404,
        ///// <summary>
        ///// 表示请求超时。
        ///// </summary>
        //RequestTimeout = 408,
        ///// <summary>
        ///// 表示请求的契约版本冲突。
        ///// </summary>
        //Conflict = 409,
        /// <summary>
        /// 表示服务发生了内部错误。
        /// </summary>
        InternalServerError = 500,
        ///// <summary>
        ///// 表示契约服务可能正在维护。
        ///// </summary>
        //ServerUnavailable = 503,
    }

}
