using System;
using System.Collections.Generic;
using System.Text;

namespace AttRest.CodeType
{

    public class ApiModel
    {
        /// <summary>
        /// 接口描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 请求方式 get/post/put/delete
        /// </summary>
        public string RequestMethod { get; set; }


        /// <summary>
        /// Route过滤
        /// </summary>
        public string RouteFilter { get; set; } = string.Empty;

        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// 请求路径名称
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 路径Route拦截
        /// </summary>
        public string ActionRouteFilter { get; set; }

        /// <summary>
        /// 是否为异步
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 参数是否有自定义类型
        /// </summary>
        public bool IsAllParamPrimitive { get; set; } = true;

        /// <summary>
        /// 返回是否为自定义类型
        /// </summary>
        public bool IsReturnPrimitive { get; set; } = true;

        /// <summary>
        /// 参数类型
        /// </summary>
        public List<ParamTypeEntity> ParamTypes { get; set; } = new List<ParamTypeEntity>();

        /// <summary>
        /// 方法返回的类型
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        /// 获取区域
        /// </summary>
        public string Area { get; internal set; }
    }
}
