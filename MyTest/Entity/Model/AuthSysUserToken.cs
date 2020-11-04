using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTest.Entity.Model
{
    public class AuthSysUserToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime? ExpireTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
