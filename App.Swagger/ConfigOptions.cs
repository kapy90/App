using System;
using System.Collections.Generic;
using System.Text;

namespace App.Swagger
{
    public class ConfigOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = false;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = "";
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; } = "";
        /// <summary>
        /// 访问令牌
        /// 不为空时启用访问令牌
        /// </summary>
        public string SwaggerLockToken { get; set; } = "";
    }
}
