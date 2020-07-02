using AttRest.CodeType;
using AttRest.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace AttRest
{
    public static class NetCoreExt
    {
        /// <summary>
        /// 当前的接口列表
        /// </summary>
        public static ApiEnumerable ApiEnumerable = default(ApiEnumerable);

        /// <summary>
        /// 拓展Net Core Services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="attCodeType"></param>
        /// <param name="ass"></param>
        public static void AddAttRest(this IServiceCollection services, AttClientFrame attCodeType, Assembly ass)
        {
            ApiEnumerable = new ApiEnumerable(attCodeType);
            //获取当前所有类
            foreach (var _type in ass.GetTypes())
            {
                if (_type.Name.EndsWith("Controller"))
                {
                    foreach (var method in _type.GetMethods())
                    {
                        ApiModel apiModel = new ApiModel();
                        var desc = method.GetCustomAttributes(typeof(AttDescription), true);
                        var get = method.GetCustomAttributes(typeof(HttpGetAttribute), true);
                        var post = method.GetCustomAttributes(typeof(HttpPostAttribute), true);
                        var put = method.GetCustomAttributes(typeof(HttpPutAttribute), true);
                        var delete = method.GetCustomAttributes(typeof(HttpDeleteAttribute), true);
                        bool isRequest = false;
                        //自动获取路由前缀，这里是NetCore代码
                        var route = _type.GetCustomAttribute(typeof(RouteAttribute));
                        apiModel.RouteFilter = (route as RouteAttribute)?.Template;
                        //自动获取区域
                        var area = _type.GetCustomAttribute(typeof(AreaAttribute));
                        apiModel.Area = (area as AreaAttribute)?.RouteValue;
                        //get
                        if (get.Length > 0)
                        {
                            apiModel.RequestMethod = "get";
                            isRequest = true;
                        }
                        //post
                        if (post.Length > 0)
                        {
                            apiModel.RequestMethod = "post";
                            isRequest = true;
                        }
                        //put
                        if (put.Length > 0)
                        {
                            apiModel.RequestMethod = "put";
                            isRequest = true;
                        }
                        //delete
                        if (delete.Length > 0)
                        {
                            apiModel.RequestMethod = "delete";
                            isRequest = true;
                        }
                        //是请求类型
                        if (isRequest)
                        {
                            apiModel.ControllerName = _type.Name.Replace("Controller", string.Empty);
                            apiModel.ActionName = method.Name;
                            var method_route = method.GetCustomAttribute(typeof(RouteAttribute));
                            apiModel.ActionRouteFilter = (method_route as RouteAttribute)?.Template;
                            apiModel.Description = desc.Length > 0 ? (string.IsNullOrEmpty(desc[0].ToString()) ? $@"接口{method.Name},无说明" : $@"接口{method.Name}:{desc[0].ToString()}") : $@"接口{method.Name},无说明";
                            //是否为异步
                            foreach (var att in method.CustomAttributes)
                            {
                                if (att.AttributeType == typeof(AsyncStateMachineAttribute))
                                {
                                    apiModel.IsAsync = true;
                                }
                            }
                            foreach (var p in method.GetParameters())
                            {
                                if (!p.ParameterType.IsPrimitive
                                    && !p.ParameterType.Assembly.FullName.Contains("System")
                                    && !p.ParameterType.Assembly.FullName.Contains("Microsoft"))
                                {
                                    //参数有自定义类型
                                    apiModel.IsAllParamPrimitive = false;
                                    //判断是否为枚举
                                    if (!p.ParameterType.IsEnum)
                                    {
                                        //获取所有公共属性,递归编译DTO
                                        var Properties = p.ParameterType.GetProperties();
                                        var pdesc = p.ParameterType.GetCustomAttributes(typeof(AttDescription), true);
                                        var Description = pdesc.Length > 0 ? (string.IsNullOrEmpty(pdesc[0].ToString()) ? $@"对象{p.ParameterType.Name},无说明" : $@"对象{p.ParameterType.Name}:{pdesc[0].ToString()}") : $@"对象{p.ParameterType.Name},无说明";
                                        GetFieldsList(p.ParameterType, Properties, ApiEnumerable, Description);
                                    }
                                    else
                                    {
                                        //是枚举类型
                                        var Fields = p.ParameterType.GetFields(BindingFlags.Static | BindingFlags.Public);
                                        //说明
                                        var pdesc = p.ParameterType.GetCustomAttributes(typeof(AttDescription), true);
                                        var Description = pdesc.Length > 0 ? (string.IsNullOrEmpty(pdesc[0].ToString()) ? $@"枚举{p.ParameterType.Name},无说明" : $@"枚举{p.ParameterType.Name}:{pdesc[0].ToString()}") : $@"枚举{p.ParameterType.Name},无说明";
                                        //建立enumModel
                                        var enumModel = new EntityModel();
                                        enumModel.ClassName = p.ParameterType.Name;
                                        enumModel.DllName = p.ParameterType.Assembly.FullName;
                                        enumModel.Namespace = p.ParameterType.Namespace;
                                        enumModel.Description = Description;
                                        foreach (var f in Fields)
                                        {
                                            var fdesc = f.GetCustomAttributes(typeof(AttDescription), true);
                                            var fDescription = fdesc.Length > 0 ? (string.IsNullOrEmpty(fdesc[0].ToString()) ? $@"枚举{f.Name},无说明" : $@"枚举{f.Name}:{fdesc[0].ToString()}") : $@"枚举{f.Name},无说明";
                                            enumModel.Parameters.Add(new FieldModel() { Name = f.Name, DefaultValue = f.GetValue(null), FieldType = typeof(object), Description = fDescription });
                                        }
                                        ApiEnumerable.EnumModels.Add(enumModel);
                                    }
                                }
                                else
                                {
                                    //判断是否内含泛型
                                    HasGeneric(ApiEnumerable, p.ParameterType);
                                }
                                apiModel.ParamTypes.Add(new ParamTypeEntity { ParamType = p.ParameterType, Name = p.Name });
                            }
                            //返回包含自定义类型
                            if (!method.ReturnType.IsPrimitive
                                && !method.ReturnType.Assembly.FullName.Contains("System")
                                && !method.ReturnType.FullName.Contains("Microsoft"))
                            {
                                apiModel.IsAllParamPrimitive = false;
                            }
                            apiModel.ReturnType = method.ReturnType;
                            ApiEnumerable.ApiModels.Add(apiModel);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否有List、IEnumerable泛型
        /// </summary>
        private static void HasGeneric(ApiEnumerable apiEnumerable, Type paramType)
        {
            if (paramType.GUID == typeof(List<>).GUID || paramType.GUID == typeof(IEnumerable<>).GUID)
            {
                var x = paramType.GetGenericArguments()[0];
                var Properties = x.GetProperties();
                var pdesc = x.GetCustomAttributes(typeof(AttDescription), true);
                var Description = pdesc.Length > 0 ? (string.IsNullOrEmpty(pdesc[0].ToString()) ? $@"对象{x.Name},无说明" : $@"对象{x.Name}:{pdesc[0].ToString()}") : $@"对象{x.Name},无说明";
                GetFieldsList(x, Properties, apiEnumerable, Description);
            }
        }

        /// <summary>
        /// 递归获取属性的字串
        /// </summary>
        /// <param name="PType"></param>
        /// <param name="Properties"></param>
        /// <param name="apiEnumerable"></param>
        /// <param name="_desc"></param>
        private static void GetFieldsList(Type PType, PropertyInfo[] Properties, ApiEnumerable apiEnumerable, string _desc)
        {
            EntityModel entity = new EntityModel();
            entity.Description = _desc;
            entity.ClassName = PType.Name;
            entity.Namespace = PType.Namespace;
            entity.DllName = PType.Assembly.FullName;
            foreach (var p in Properties)
            {

                if (!p.PropertyType.IsPrimitive
                    && !p.PropertyType.Assembly.FullName.Contains("System")
                    && !p.PropertyType.Assembly.FullName.Contains("Microsoft"))
                {
                    //非系统属性对象
                    var descsub = p.PropertyType.GetCustomAttributes(typeof(AttDescription), true);
                    var subDescription = descsub.Length > 0 ? (string.IsNullOrEmpty(descsub[0].ToString()) ? $@"对象{p.Name},无说明" : $@"对象{p.Name}:{descsub[0].ToString()}") : $@"对象{p.Name},无说明";
                    GetEntityFieldsList(p.PropertyType, p.PropertyType.GetProperties(), apiEnumerable, subDescription);
                }
                var desc = p.GetCustomAttributes(typeof(AttDescription), true);
                var Description = desc.Length > 0 ? (string.IsNullOrEmpty(desc[0].ToString()) ? $@"参数{p.Name},无说明" : $@"参数{p.Name}:{desc[0].ToString()}") :
                    $@"参数{p.Name},无说明";
                entity.Parameters.Add(new FieldModel { Name = p.Name, FieldType = p.PropertyType, Description = Description });

            }
            if (apiEnumerable.EntityModels.FindAll(e => e.ClassName == entity.ClassName && e.Namespace == entity.Namespace && e.DllName == entity.DllName).Count < 1)
            {
                apiEnumerable.EntityModels.Add(entity);
            }
        }

        private static void GetEntityFieldsList(Type EntityType, PropertyInfo[] Properties, ApiEnumerable apiEnumerable, string _desc)
        {
            EntityModel entity = new EntityModel();
            entity.Description = _desc;
            entity.ClassName = EntityType.Name;
            entity.Namespace = EntityType.Namespace;
            entity.DllName = EntityType.Assembly.FullName;
            foreach (var p in Properties)
            {

                if (!p.PropertyType.IsPrimitive
                    && !p.PropertyType.Assembly.FullName.Contains("System")
                    && !p.PropertyType.Assembly.FullName.Contains("Microsoft"))
                {
                    //非系统属性对象
                    var descsub = p.PropertyType.GetCustomAttributes(typeof(AttDescription), true);
                    var subDescription = descsub.Length > 0 ? (string.IsNullOrEmpty(descsub[0].ToString()) ? $@"对象{p.Name},无说明" : $@"对象{p.Name}:{descsub[0].ToString()}") : $@"对象{p.Name},无说明";
                    GetEntityFieldsList(p.PropertyType, p.PropertyType.GetProperties(), apiEnumerable, subDescription);
                }
                var desc = p.GetCustomAttributes(typeof(AttDescription), true);
                var Description = desc.Length > 0 ? (string.IsNullOrEmpty(desc[0].ToString()) ? $@"参数{p.Name},无说明" : $@"参数{p.Name}:{desc[0].ToString()}") :
                    $@"参数{p.Name},无说明";
                entity.Parameters.Add(new FieldModel { Name = p.Name, FieldType = p.PropertyType, Description = Description });

            }
            if (apiEnumerable.EntityModels.FindAll(e => e.ClassName == entity.ClassName && e.Namespace == entity.Namespace && e.DllName == entity.DllName).Count < 1)
            {
                apiEnumerable.EntityModels.Add(entity);
            }
        }
    }
}
