using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// 对路由的解析
/// </summary>
namespace AttRest.CodeType
{
    public class RouteAnaly
    {

        private string routeFilter;
        private string controllerName;
        private string actionName;
        private string actionRouteFilter;
        private string areaFilter = "";

        /// <summary>
        /// 是否为伪静态链接
        /// </summary>
        public bool HasRewrite { get; set; } = false;


        /// <summary>
        /// 链接的地址
        /// </summary>
        public string Link { get;private set; }

        /// <summary>
        /// 链接的地址显示
        /// </summary>
        public string LinkShow { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="RouteFilter"></param>
        public RouteAnaly(string Area,string RouteFilter, string ControllerName, string ActionName, string ActionRouteFilter)
        {
            areaFilter = string.IsNullOrEmpty(Area) ? "" : Area.ToLower();
            routeFilter = string.IsNullOrEmpty(RouteFilter) ? "" : RouteFilter.ToLower();
            actionName = ActionName;
            controllerName = ControllerName;
            actionRouteFilter = ActionRouteFilter;
            Init();
        }

        /// <summary>
        /// 转为真实的URL
        /// </summary>
        /// <returns></returns>
        private void Init() {

            if (string.IsNullOrEmpty(routeFilter))
            {
                if (string.IsNullOrEmpty(actionRouteFilter))
                {
                    //route为空，则直接使用controller
                    Link = $@"/{actionName}";
                }
                else
                {
                    //判断URL中的参数
                    if (Regex.IsMatch(actionRouteFilter, @"\{[_a-z A-Z 0-9]*(\?)*\}"))
                    {
                        HasRewrite = true;
                    }
                    Link = $@"/{actionRouteFilter}";
                }
            }
            //匹配只有controller的情况
            else if (Regex.IsMatch(routeFilter, @"([a-z A-Z 0-9]*\/)*\[controller\]$"))
            {
                //该匹配到controller,则只有一个方法的匹配
                Link = routeFilter.Replace("[controller]", $@"{controllerName}");//$@"/{controllerName}";
            }
            else if (Regex.IsMatch(routeFilter, @"([a-z A-Z 0-9]*\/)*\[controller\]\/\[action\]$"))
            {
                //直接匹配到action
                Link = routeFilter.Replace("[controller]", $@"{controllerName}").Replace("[action]", $@"{actionName}");
                if (!string.IsNullOrEmpty(actionRouteFilter))
                {
                    if (actionRouteFilter.StartsWith("/"))
                    {
                        Link += actionRouteFilter.Replace("[action]", $@"{actionName}");
                    }
                    else
                    {
                        Link += "/" + actionRouteFilter.Replace("[action]", $@"{actionName}");
                    }
                }
            }
            else {
                Link = routeFilter.Replace("[controller]", $@"{controllerName}").Replace("[action]", $@"{actionName}");
            }
            //加上开头的斜杠,以及区域
            if (!string.IsNullOrEmpty(Link) && !Link.StartsWith("/")) {
                Link =  "/" + Link;
            }
            Link = Link.Replace("[Area]", areaFilter, StringComparison.CurrentCultureIgnoreCase);
            //设置展示的Link
            LinkShow = Link;
            //去除Link中的{x}的变量
            Regex rx = new Regex(@"{([a-z A-Z 0-9]*[:]?)*}");
            if (rx.IsMatch(Link))
            {
                var paramSeg = rx.Match(Link).Value.Replace("{",string.Empty).Replace("}", string.Empty);
                var paramName = "";
                if (paramSeg.Contains(":"))
                {
                    paramName = paramSeg.Split(":")[0];
                }
                else { 
                    paramName = paramSeg;
                }
                Link = Regex.Replace(Link, rx.Match(Link).Value ,"'+" + paramName +"+'");
            }
            Link = "'" +  Link + "'";
        }
    }
}
