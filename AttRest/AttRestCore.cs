using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace AttRest
{
    public class AttRestCore
    {
        /// <summary>
        /// 获取API-DOM结构
        /// </summary>
        /// <param name="Host"></param>
        /// <returns></returns>
        public static string Schema(string Host = "localhost:5001") {
            return NetCoreExt.ApiEnumerable.ToSchema(Host);
        }

        /// <summary>
        /// 获取API-DOM结构网页默认的Css样式
        /// </summary>
        /// <returns></returns>
        public static string DefaultCss() { 
             return NetCoreExt.ApiEnumerable.GetDefaultCss();
        }

        /// <summary>
        /// 获取客户端可拉取的代码,
        /// 代码类型根据服务端启动类初始的设置，如Vue,React,Angular等
        /// </summary>
        /// <returns></returns>
        public static void ResponseScript(HttpContext HttpContext) {
            ResponseScript(HttpContext , Encoding.GetEncoding("gbk"));
        }

        /// <summary>
        /// 获取客户端可拉取的代码
        /// </summary>
        /// <returns></returns>
        public static void ResponseScript(HttpContext HttpContext, Encoding encoding)
        {
            //生成代码
            var code = NetCoreExt.ApiEnumerable.ToCode();
            //向浏览器返回HttpResponse
            var Response = HttpContext.Response;
            Response.ContentType = "text/javascript"; //js类型
            var contentDisposition = "attachment;" + "filename=" + HttpUtility.UrlEncode("AttRestClient.js");//在Response的Header中设置下载文件的文件名，这样客户端浏览器才能正确显示下载的文件名，注意这里要用HttpUtility.UrlEncode编码文件名，否则有些浏览器可能会显示乱码文件名
            Response.Headers.Add("Content-Disposition", new string[] { contentDisposition });
            var retcode = encoding.GetBytes(code);
            long hasRead = 0;//变量hasRead用于记录已经发送了多少字节的数据到客户端浏览器
                             //如果hasRead小于contentLength，说明下载文件还没读取完毕，继续循环读取下载文件的内容，并发送到客户端浏览器
            using (Response.Body)
            {
                while (hasRead < retcode.Length)
                {
                    //HttpContext.RequestAborted.IsCancellationRequested可用于检测客户端浏览器和ASP.NET Core服务器之间的连接状态，如果HttpContext.RequestAborted.IsCancellationRequested返回true，说明客户端浏览器中断了连接
                    if (HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        //如果客户端浏览器中断了到ASP.NET Core服务器的连接，这里应该立刻break，取消下载文件的读取和发送，避免服务器耗费资源
                        break;
                    }
                    var needRead = 1024;
                    if (retcode.Length - hasRead < 1024)
                    {
                        needRead = retcode.Length - Convert.ToInt32(hasRead);
                    }
                    Response.Body.WriteAsync(retcode, Convert.ToInt32(hasRead), Convert.ToInt32(needRead));//发送读取的内容数据到客户端浏览器
                    Response.Body.FlushAsync();//注意每次Write后，要及时调用Flush方法，及时释放服务器内存空间
                    hasRead += needRead;//更新已经发送到客户端浏览器的字节数
                }
            }
        }

        public static void ToMakeDown() { 
        }
    }
}
