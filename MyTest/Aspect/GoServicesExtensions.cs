using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyTest.Config;
using MyTest.Entity;
using MyTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTest.Aspect
{
    public static class GoServicesExtensions
    {
        public static IServiceCollection AddGoServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);
            //添加对控制器以及与 API 相关的功能
            services.AddControllers();

            //跨域
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                    .SetIsOriginAllowed(t => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            services.AddDbContext<AuthDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("passport"));
            });
            //登录用户
            services.AddScoped<LoginUser>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //注入配置类
            services.Configure<GlobalSettings>(configuration.GetSection("GlobalSettings"));
            //鉴权服务
            services.AddScoped<IAuthService, OauthAuthServiceImpl>();
            services.AddScoped<IAuthService, JwtAuthServiceImpl>();
            //ServiceLocator.SetLocatorProvider(services.BuildServiceProvider());
            return services;
        }
    }
}
