using Microsoft.AspNetCore.Http;
using MyTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTest.Middlerware
{
    public sealed class LoginMiddlerware
    {
        private readonly RequestDelegate _next;

        public LoginMiddlerware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 设置登录用户
        /// </summary>
        /// <param name="context"></param>
        /// <param name="_authService"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, IEnumerable<IAuthService> _authService)
        {
            string token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                if (token.StartsWith("Bearer") || token.Contains('.'))
                {
                    await _authService.First(a => a.ServiceName == nameof(JwtAuthServiceImpl)).SetUserAsync(token);
                }
                else
                {
                    await _authService.First(a => a.ServiceName == nameof(OauthAuthServiceImpl)).SetUserAsync(token);
                }
            }
            await _next.Invoke(context);
            string GetToken()
            {
                string token = context.Request.Headers["token"];
                if (string.IsNullOrEmpty(token))
                {
                    token = context.Request.Query["token"];
                }
                return token;
            }
        }
    }
}
