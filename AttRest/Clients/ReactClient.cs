﻿

namespace AttRest.Clients
{
    using AttRest.CodeType;
    using AttRest.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ReactClient : IClientCode
    {
        //表示代码的域
        private string Area { get; set; } = "";

        public string ToCode()
        {
            var api = ToAPI();
            var _enum = ToEnum();
            var entity = ToEntity();
            var returnOjb = $@" return function(){{this.React = that.React;}}";
            //外部类
            string _Class = $@" this.React = function(react,axios){{ react.$api = {{}}; react.$enum = {{}};  react.$entity= {{}};   var that = this;    that.ErrorCatch = null; 
                {api}                
                {_enum}
                {entity}
                {returnOjb}
            }};";
            return _Class;
        }

        /// <summary>
        /// 生成自定义对象
        /// </summary>
        /// <returns></returns>
        private string ToEntity()
        {
            //定义对象
            var func = "";
            foreach (var model in NetCoreExt.ApiEnumerable.EntityModels)
            {
                //原型方式创建js对象
                var entityjs = $@"/** @description 实体对象[{model.Description}]  */ react.$entity.{model.ClassName}= {{}};";
                var func_body = "";
                foreach (var param in model.Parameters)
                {
                    var defaultValue = "null";
                    //自定义类型
                    if (!param.FieldType.IsPrimitive && !param.FieldType.Assembly.FullName.Contains("System") && !param.FieldType.FullName.Contains("Microsoft"))
                    {
                        defaultValue = $@"new {param.FieldType.Name}()";
                    }
                    //系统类型
                    else if (param.FieldType.GUID == typeof(int).GUID || param.FieldType.GUID == typeof(long).GUID || param.FieldType.GUID == typeof(float).GUID || param.FieldType.GUID == typeof(decimal).GUID || param.FieldType.GUID == typeof(uint).GUID)
                    {
                        defaultValue = "0";
                    }
                    else if (param.FieldType.GUID == typeof(IEnumerable<>).GUID || param.FieldType.GUID == typeof(List<>).GUID)
                    {
                        defaultValue = "[]";
                    }
                    else if (param.FieldType.GUID == typeof(string).GUID)
                    {
                        defaultValue = "''";
                    }
                    else if (param.FieldType.GUID == typeof(DateTime).GUID || param.FieldType.GUID == typeof(DateTime?).GUID)
                    {
                        defaultValue = "new Date()";
                    }
                    func_body += $@"/** @description {param.Description}    @example in react : this.$entity.{model.ClassName}.{param.Name}; */ react.$entity.{model.ClassName}.{param.Name} = {defaultValue};";
                }
                func += entityjs + func_body;
            }
            return func;
        }

        /// <summary>
        /// 生成枚举代码
        /// </summary>
        /// <returns></returns>
        private string ToEnum()
        {
            //定义枚举
            var func = "";

            foreach (var model in NetCoreExt.ApiEnumerable.EnumModels)
            {
                var enumjs = $@"/** @description 枚举[{model.Description}]  */ react.$enum.{model.ClassName} = new Object();";
                var func_body = "";
                foreach (var param in model.Parameters)
                {
                    var defaultValue = Convert.ToInt32(param.DefaultValue);
                    func_body += $@"/** @description {param.Description}    @example react: this.$enum.{model.ClassName}.{param.Name};   */ react.$enum.{model.ClassName}.{param.Name} = {defaultValue}; ";
                }
                enumjs = enumjs + func_body;
                //所有的初始化
                func += enumjs;
            }
            return func;
        }

        /// <summary>
        /// 生成接口代码
        /// </summary>
        /// <returns></returns>
        private string ToAPI()
        {
            var Host = NetCoreExt.Host; //临时测试使用
            IEnumerable<IGrouping<string, ApiModel>> query = NetCoreExt.ApiEnumerable.ApiModels.GroupBy(api => api.ControllerName);
            var react_api = "";
            var react_api_list = "";
            foreach (var coll in query)
            {
                react_api_list += $@"'{coll.First().ControllerName.Replace("Controller", string.Empty)}',";
                List<ApiModel> api = coll.ToList<ApiModel>();
                this.Area = string.IsNullOrEmpty(api.FirstOrDefault().Area) ? "" : api.FirstOrDefault().Area + "_";
                var methods = "";
                foreach (var method in api)
                {
                    RouteAnaly routeAnaly = new RouteAnaly(method.Area, method.RouteFilter, method.ControllerName, method.ActionName, method.ActionRouteFilter);

                    var link = $@"'{Host}' + {routeAnaly.Link}";
                    //参数整理
                    var _params = "";
                    var data = "";
                    foreach (var p in method.ParamTypes)
                    {
                        _params += $@"{p.Name},";
                        data += $@"{p.Name}:{p.Name},";
                    }
                    if (data.Length > 0)
                    {
                        data = data.Substring(0, data.Length - 1);
                        data = $@"{{{data}}}";
                    }
                    else
                    {
                        data = "{}";
                    }
                    var methodRequestMethod = method.RequestMethod.ToLower();
                    //头部，暂时的方法
                    var headers = methodRequestMethod.ToLower() == "get" ? "" : @" headers:{{ 'Content-Type': 'application/x-www-form-urlencoded' }}";
                    var dataType = methodRequestMethod.ToLower() == "get" ? "params" : "data";
                    //API方法构建
                    var react_method = $@"    {method.ActionName}:function({_params}callback){{
                    axios({{
                        ""method"":'{methodRequestMethod}'
                        ""url"":{link}
                        {dataType}:{data},
                        {headers}
                    }}).catch(function(error){{
                        if(that.ErrorCatch!=null && that.ErrorCatch!=undefined){{
                            that.ErrorCatch(error,""${coll.First().ControllerName.Replace("Controller", string.Empty)}-{method.ActionName}"");
                        }} 
                    }}).then(function(res){{
                        callback(res);
                    }})}},";
                    methods += react_method;
                }
                methods = methods.Substring(0, methods.Length - 1);
                react_api += $@"  react.$api.{this.Area}{coll.First().ControllerName.Replace("Controller", string.Empty)} = {{{methods}}},";
            }
            react_api_list = react_api_list.Substring(0, react_api_list.Length - 1);
            react_api = react_api.Substring(0, react_api.Length - 1);
            return react_api;
        }
    }
}
