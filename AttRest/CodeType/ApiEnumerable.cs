using AttRest.Enum;
using AttRest.Interface;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AttRest.CodeType
{
    public class ApiEnumerable
    {
        private AttClientFrame attCodeType ;

        private string logo64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAADc0lEQVRoQ2NkcJ/7n2EYAMZRjwyyWByNkUEWIQyjMTIaIzQKgdGkRaOAJdvYkRkj8Ro8DBPKbbGG2obdNxkSl90jO0Qp1UhSjKxKVmNwtVHGaufNe68ZLJrPUOoesvWT5JGb3bYMYiI8OC0r6DzMsPDGF7IdQ4lGoj1SYi7EUJ1hjteu3UfuMoTNvUWJe8jWS7RHtuZoM1gZy8Et+vnrD5jNzsYCF3v15guDeulhsh1DiUaiPfJwkiMDHy8H3K4L156D2QZakij2D1TyIsojk/xlGGIDdFEcvHjDZTAfXZyY5FX19xOD0L/fDO+YWBnamPnwRgSxaonyyP5SA5SQByUrifTdDC7ibAyrO5xRHPLp8w8G+bz9OB235cddBo9vu+HyhzntGRw5NbGqJ0UtQY+AHLukyR4lL4CSlWP3BbDlJ2pNGNSVRFEc0jrjJEPPyXdYHff240YG/r+QZAkDLEIZFKsl6JH5UUoMAa7qGMkqb+MTsBg2+WNnHzF4T7mK1XF/3s3AEMflEVLUEvQIeojDkhXMNaQmL1IcR4pavB7B1iRBTlYwz1xssWSQkxZACempS88x1Ox5CRbr+f2WIfPLTgb2/5/wZuyXLEoMyzmNiVYrzecGNw+vR7A1SUClFSxZwUzBpg7Zw+iZFp9vdnC5ohQG+NTW8UfBSz28HkFvkqAnK5gl2GIOWW3qv+8MHZ93YWRydEc+YNVi6OQ2JVqtCq8d4RjB5jh8mRhbOwxb7JGS7klRizNG0JskeBM3Dkls+YkUx5GiFqdH0Jsk5HgElLxi6g4y7Hn5C66dFMeRoharR1pcxBmyo43IcTuGHvQOFymOI0UtVo+gN0ko8RF6h4sUx5GiFqtHXsx0RWmS4Cqt0D2Iq88SWrEXnrxIcRwpajE8gq3JgS3T4oolbKUXcvLC5jhQ3fGFkY1hL6sIw2wmTrz5CZdaDI+Q2ghE9xC2yhG5w3X5yykGzV/nsIbDT0Y+Bm7BKLgcKWpRPEJquwmba3CNtMA6XKDmSsHn1TizHXJtTYpaFI9gC01iOkrorsIWq8jmzP71gsHrxyUG8T+I4SNQbOznNGfw4UAdpSFWLcHWLyUlFj31jnqEnqFNjF2jMUJMKNFTzWiM0DO0ibFrNEaICSV6qhmNEXqGNjF2jcYIMaFETzWjMULP0CbGLgDmcfWYVXNodQAAAABJRU5ErkJggg==";

        /// <summary>
        /// 根据不同的类别生成代码
        /// </summary>
        /// <param name="attCodeType"></param>
        public ApiEnumerable(AttClientFrame attCodeType)
        {
            this.attCodeType = attCodeType;

        }


        /// <summary>
        /// 所有方法
        /// </summary>
        public List<ApiModel> ApiModels { get; set; } = new List<ApiModel>();

        /// <summary>
        /// 所有需要重新编译的Entity
        /// </summary>
        public List<EntityModel> EntityModels { get; set; } = new List<EntityModel>();

        /// <summary>
        /// 所有需要重新编译的枚举
        /// </summary>
        public List<EntityModel> EnumModels { get; set; } = new List<EntityModel>();

        /// <summary>
        /// 根据不同类型生成代码
        /// </summary>
        public string ToCode()
        {
            var codeClient = ClientCodeFactory.Instance.GetCodeBuilder(attCodeType);
            var retstr = "";
            foreach (var client in codeClient)
            {
                retstr += client.ToCode();
            }
            return "function AttClient(){"+retstr+"}";
        }

        /// <summary>
        /// 获取API的整体架构描述
        /// </summary>
        /// <param name="RouteName"></param>
        /// <param name="Host"></param>
        /// <returns></returns>
        public string ToSchema(string Host)
        {
            Host = "https://" + Host + "";
            //定义枚举
            var func = $@"";
            foreach (var model in EnumModels)
            {
                var enumjs = $@"<span class='EnumName'>{model.ClassName}(枚举)</span><span class='EnumDetail'>.Net Core描述:[{model.Description}]</span>";
                var func_body = "";
                foreach (var param in model.Parameters)
                {
                    var defaultValue = Convert.ToInt32(param.DefaultValue);
                    func_body += $@"<ul class='EnumParam'><li>参数名称: {param.Name} </li> <li> {param.Description}</li> <li>示例： var x = {model.ClassName}.{param.Name};</li> <li>编译值:{defaultValue}</li></ul> ";
                }
                enumjs = enumjs + func_body;
                //所有的初始化
                func += $@"<li>{enumjs}</li>";
            }
            func = $@"<p class='title_p'>AttRest-Document:[{Host}]<img class='logo' src=""{logo64}""></img></p><ul class='enum_ul'>{func}</ul>";
            //定义对象
            foreach (var model in EntityModels)
            {
                //原型方式创建js对象
                var entityjs = $@"<span  class='ModelName'>{model.ClassName}(实体对象)</span><span  class='ModelDetail'>.Net Core描述:[{model.Description}]</span>";
                var func_body = "";
                foreach (var param in model.Parameters)
                {
                    var defaultValue = "null";
                    //自定义类型
                    if (!param.FieldType.IsPrimitive && !param.FieldType.Assembly.FullName.Contains("System") && !param.FieldType.FullName.Contains("Microsoft"))
                    {
                        defaultValue = $@"自定义类型:{param.FieldType.Name}";
                    }
                    //系统类型
                    else if (param.FieldType.GUID == typeof(int).GUID || param.FieldType.GUID == typeof(long).GUID || param.FieldType.GUID == typeof(float).GUID || param.FieldType.GUID == typeof(decimal).GUID || param.FieldType.GUID == typeof(uint).GUID)
                    {
                        defaultValue = "数字类型";
                    }
                    else if (param.FieldType.GUID == typeof(IEnumerable<>).GUID || param.FieldType.GUID == typeof(List<>).GUID)
                    {
                        defaultValue = "数组";
                    }
                    else if (param.FieldType.GUID == typeof(string).GUID)
                    {
                        defaultValue = "字符串";
                    }
                    else if (param.FieldType.GUID == typeof(DateTime).GUID || param.FieldType.GUID == typeof(DateTime?).GUID)
                    {
                        defaultValue = "时间日期";
                    }
                    func_body += $@"<ul class='ModelParam'><li>参数名称: {param.Name} </li> <li> {param.Description}</li> <li>示例： var x = new {model.ClassName}; console.log(x.{param.Name});</li> <li>传入类型:{defaultValue}</li></ul> ";
                }
                entityjs = entityjs + func_body;
                func += $@"<ul  class='entity_ul'>{entityjs}</ul>";
            }
            //定义接口对象
            //Linq分组处理          
            IEnumerable<IGrouping<string, ApiModel>> query = ApiModels.GroupBy(api => api.ControllerName);
            foreach (var coll in query)
            {

                List<ApiModel> api = coll.ToList<ApiModel>();
                //原型方式创建js-api对象
                var apijs = $@"<span class='apiName'>{api.First().ControllerName}(API对象)</span>";
                //apijs += $@"<span class='apiDetail'>.Net Core描述:[{api.First().Description}]</span>";
                var func_body = "";
                foreach (var method in api)
                {
                    var _params = "";
                    RouteAnaly routeAnaly = new RouteAnaly(method.Area,method.RouteFilter, method.ControllerName, method.ActionName, method.ActionRouteFilter);
                    var url =  $@"{{Host}}{routeAnaly.LinkShow}";
                    var link = $@"{Host}{routeAnaly.LinkShow}";
                    //参数整理
                    foreach (var p in method.ParamTypes)
                    {
                        var typeStr = "";
                        //自定义类型
                        if (!p.ParamType.IsPrimitive && !p.ParamType.Assembly.FullName.Contains("System") && !p.ParamType.FullName.Contains("Microsoft"))
                        {
                            typeStr = $@"自定义类型:{p.ParamType.Name}";
                        }
                        //系统类型
                        else if (p.ParamType.GUID == typeof(int).GUID || p.ParamType.GUID == typeof(long).GUID || p.ParamType.GUID == typeof(float).GUID || p.ParamType.GUID == typeof(decimal).GUID || p.ParamType.GUID == typeof(uint).GUID)
                        {
                            typeStr = "数字类型";
                        }
                        else if (p.ParamType.GUID == typeof(IEnumerable<>).GUID || p.ParamType.GUID == typeof(List<>).GUID)
                        {
                            typeStr = "数组";
                        }
                        else if (p.ParamType.GUID == typeof(string).GUID)
                        {
                            typeStr = "字符串";
                        }
                        else if (p.ParamType.GUID == typeof(DateTime).GUID || p.ParamType.GUID == typeof(DateTime?).GUID)
                        {
                            typeStr = "时间日期";
                        }
                        _params += $@"<li class='apiParamUnit'>参数{p.Name}</li><li class='apiParamUnit'>传入类型:{typeStr}</li>";
                    }
                    func_body += $@"<ul class='apiParam'><li class='apiUnit'>方法名称:{method.ActionName}</li><li class='apiUnit'><span>请求方式:{method.RequestMethod}</span></li><li class='apiUnit'>请求地址:<a target='_blank' href='{link}'>{url}</a><li class='apiDesc'>.Net Core描述:[{method.Description}]<li/></li>{_params}</ul>";
                }
                //添加到代码返回
                apijs = apijs + func_body;
                func += $@"<ul class='func_ul'>{apijs}</ul>";
            }

            return $@"<body>{func}</body>";
        }


        /// <summary>
        /// 获取Schema页面基础CSS央视
        /// </summary>
        /// <returns></returns>
        public string GetDefaultCss() {
            #region Default-CSS
            var retcss = "*{margin:0;padding:0;}strong{font-weight:bold;color:#ff6600;font-size:14px;}body{font-family:\"Arial\",\"Microsoft YaHei\",\"宋体\",\"宋体\",sans-serif;background:#112233;}.logo{float: left;width: 35px;height: 35px;margin: 4 4 0 4;border-radius: 4px;}.title_p{display:block;width:1080px;margin:0 auto;font-size:25px;text-indent:10px;color:#222;background:deepskyblue;height:42px;line-height:42px;}ul,li{list-style:none;}.enum_ul{display:block;width:1080px;margin:0 auto;height:auto;}.enum_ul > li{float:left;display:block;width:100%;margin:5px auto;}.enum_ul,.entity_ul,.func_ul{padding-bottom:30px;}.enum_ul:after,.entity_ul:after,.func_ul:after{content:\"\";display:block;width:100%;height:0px;clear:both;}.entity_ul{display:block;width:1080px;margin:0 auto;height:auto;}.entity_ul > li{float:left;display:block;width:100%;margin:5px auto;}.func_ul{display:block;width:1080px;margin:0 auto;height:auto;}.func_ul > li{float:left;display:block;width:100%;margin:0 auto;}.EnumName{width:100%;display:block;font-weight:800;line-height:40px;color:#FEFEFE;}.ModelName{width:100 %;display:block;line-height:40px;font-weight:800;color:#0094ff;}.EnumDetail,.ModelDetail,.apiDesc{width:100%;display:block;font-weight:300;font-size:14px;color:#EEE;margin-bottom:15px;}.EnumParam,.ModelParam{width:100%;display:block;}.ModelParam,.EnumParam{padding:15px 0px;border-bottom:1px solid #ededed;}.ModelParam:after,.EnumParam:after,.apiParam:after{content:\"\";display:block;width:100%;height:0px;clear:both;}.ModelParam > li,.EnumParam > li{float:left;width:25%;font-size:11px;display:block;color:#FFFF16;font-size:12px;line-height:18px;}.ModelParam > li:nth-child(3),.EnumParam > li:nth-child(3){width:40%;}.ModelParam > li:nth-child(4),.EnumParam > li:nth-child(4){width:10%;}.apiName{width:100%;display:block;font-weight:800;line-height:40px;color:#14a100;}.apiParam{width:100%;display:block;border-top:1px solid #999;}.apiParamUnit{float:left;width:50%;font-size:11px;display:block;border-top:1px solid #ededed;border-bottom:1px solid #ededed;line-height:25px;color:#AAA;}.apiUnit{float:left;width:33%;color:#EDEDED;display:block;line-height:15px;font-weight:800;line-height:30px;font-size:12px;padding:5px 0px;white-space:pre-wrap;word-break:break-all;word-wrap:break-word;}.apiUnit > a{margin-left:5px;color:#C07af9}.apiUnit:nth-child(2){width:16%;}.apiUnit:nth-child(3){width:50%;}.bottom{width:100%;height:30px;bottom:0;position:fixed;}.footer{height:100px;bottom:0;display:table-cell;width:100 %;float:left;text-align:center;font-size:10px;color:#666;vertical-align:bottom;}.footer > p{position:relative;bottom:0px;padding:0px;margin-top:80px}";
            #endregion
            return $@"<style type=""text/css"">{retcss}</style></head>";
        }
    }
}
