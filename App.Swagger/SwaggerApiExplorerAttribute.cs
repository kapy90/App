using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;

namespace App.Swagger
{
    /// <summary>
    /// 自定义路由 /api/{groupName=v1}/[controler]/[action]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SwaggerApiExplorerAttribute : RouteAttribute, IApiDescriptionGroupNameProvider, IApiDescriptionVisibilityProvider
    {
        /// <summary>
        /// 自定义路由构造函数
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="ignoreApi">是否显示api, true是不显示，否则显示</param>
        public SwaggerApiExplorerAttribute(string groupName = "", bool ignoreApi = false) : base("/[controller]/[action]")
        {
            GroupName = groupName;
            IgnoreApi = ignoreApi;
        }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 是否显示api
        /// true是不显示，否则显示
        /// </summary>
        public bool IgnoreApi { get; set; }
    }
}
