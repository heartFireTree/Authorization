using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MyTest.Config;
using MyTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyTest.Middlerware
{
    public class ClaimRequirementAttribute
    {
        /// <summary>
        /// 自定义授权验证特性
        /// </summary>
        public class RequiresPermissionsAttribute : TypeFilterAttribute
        {
            public RequiresPermissionsAttribute(ClaimType claimType, string claimValue = "") : base(typeof(ClaimRequirementFilter))
            {
                Arguments = new object[] { new Claim(claimType.ToString(), claimValue) };
            }
        }

        /// <summary>
        /// 自定义授权验证过滤器
        /// </summary>
        public class ClaimRequirementFilter : IAuthorizationFilter
        {
            //授权声明
            readonly Claim _claim;
            //授权接口
            readonly IEnumerable<IAuthService> _authService;
            //全局配置类
            private readonly GlobalSettings _globalSettings;

            //构造函数注入
            public ClaimRequirementFilter(Claim claim, IEnumerable<IAuthService> authService, IOptions<GlobalSettings> _options)
            {
                _claim = claim;
                _authService = authService;
                _globalSettings = _options.Value;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                //获取控制器描述符
                ControllerActionDescriptor controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (controllerActionDescriptor != null)
                {
                    var skipAuthorization = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                        .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)));
                    //如果存在描述符则跳过验证 比如【AllowAnonymous】
                    if (skipAuthorization)
                    {
                        return;
                    }
                }

                //检查白名单
                if (_globalSettings.WhiteList != null && _globalSettings.WhiteList.Any(path => path == context.HttpContext.Request.Path.Value))
                {
                    return;
                }
                //声明类型
                ClaimType claimType = Enum.Parse<ClaimType>(_claim.Type);
                bool permission = false;
                //获取token
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    //返回401
                    context.Result = new UnauthorizedResult();
                    return;
                }
                if (claimType == ClaimType.Multiple)
                {
                    //根据Token类型选择认证方式
                    if (token.Any(t => t == '.'))
                    {
                        claimType = ClaimType.JWT;
                    }
                    else
                    {
                        claimType = ClaimType.Oauth2;
                    }
                }

                switch (claimType)
                {
                    case ClaimType.Oauth2:
                        permission = _authService.First(a => a.ServiceName == nameof(OauthAuthServiceImpl)).PermissionAsync(token, _claim.Value).Result;
                        break;
                    case ClaimType.JWT:
                        permission = _authService.First(a => a.ServiceName == nameof(JwtAuthServiceImpl)).PermissionAsync(token, _claim.Value).Result;
                        break;
                    default:
                        throw new Exception($"没有指定的授权方式：{claimType}");
                }

                if (!permission)
                {
                    //返回401
                    context.Result = new UnauthorizedResult();
                    return;
                }

                string GetToken()
                {
                    string token = context.HttpContext.Request.Headers["token"];
                    if (string.IsNullOrEmpty(token))
                    {
                        token = context.HttpContext.Request.Query["token"];
                    }
                    if (string.IsNullOrEmpty(token))
                    {
                        context.HttpContext.Request.Cookies.TryGetValue("token", out token);
                    }
                    return token;
                }
            }
        }
    }

    public enum ClaimType
    {
        Oauth2,
        JWT,
        Cookie,
        Multiple
    }
}
