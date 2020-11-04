using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyTest.Middlerware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTest.Aspect
{
    public static class GoAppExtensions
    {
        public static IApplicationBuilder UseGo(this IApplicationBuilder app, IConfiguration configuration)
        {
            //允许body重用
            app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });

            //设置基础路由ContextPath
            var config = configuration.GetValue<string>("GlobalSettings:ContextPath");

            if (config != null)
            {
                app.UsePathBase(new Microsoft.AspNetCore.Http.PathString($"/{config}"));
            }
            //路由匹配中间件，找到匹配的终结者路由Endpoint
            app.UseRouting();
            //启用 跨域 中间件
            app.UseCors();
            //登录中间件
            app.UseMiddleware(typeof(LoginMiddlerware));
            //针对 UseRouting 中间件中匹配到的路由进行拦截 做授权验证操作
            app.UseAuthorization();
            //终结者路由,针对 UseRouting 中间件匹配到的路由进行 委托方法的执行等操作
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            return app;
        }
    }
}
