using AttRest.Clients;
using AttRest.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace AttRest.Interface
{
    public class ClientCodeFactory
    {
        private static ClientCodeFactory _instance = null;
        public static ClientCodeFactory Instance => _instance ?? (_instance = new ClientCodeFactory());
        private ClientCodeFactory() { }


        /// <summary>
        /// 获取代码生成类
        /// </summary>
        /// <param name="attCodeType"></param>
        /// <returns></returns>
        public List<IClientCode> GetCodeBuilder(AttClientFrame attCodeType) {
            List<IClientCode> codes = new List<IClientCode>();
            if ((attCodeType & AttClientFrame.Vue) == AttClientFrame.Vue) codes.Add(new VueClient());
            if ((attCodeType & AttClientFrame.React) == AttClientFrame.React) codes.Add(new ReactClient());
            if ((attCodeType & AttClientFrame.JQuery) == AttClientFrame.JQuery) codes.Add(new JqueryClient());
            if ((attCodeType & AttClientFrame.Angular2) == AttClientFrame.Angular2) codes.Add(new AngularClient());
            if ((attCodeType & AttClientFrame.Avalon) == AttClientFrame.Avalon) codes.Add(new AvalonClient());
            return codes;
        }
    }
}
