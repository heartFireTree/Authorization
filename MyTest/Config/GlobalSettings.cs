using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTest.Config
{
    public partial class GlobalSettings
    {
        /// <summary>
        /// 全局请求前缀
        /// </summary>
        public string ContextPath { get; set; }
        /// <summary>
        /// 接口白名单
        /// </summary>
        public string[] WhiteList { get; set; }
        /// <summary>
        /// JWT
        /// </summary>
        public JwtSettings Jwt { get; set; }

        public class JwtSettings
        {
            public string Secret { get; set; }
            public long Expire { get; set; }
        }
    }
}
