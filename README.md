项目介绍
=====
GoRun
------
这个项目是一个控制台项目，主要就是模拟客户端做JWT身份效验
依赖与MyTest项目

MyTest
------
该项目是基于.net core 3.1的一个提供web API服务的项目，主要介绍一下相关文件夹
\Aspect 存放静态扩展文件，扩展Startup类的服务配置和应用程序中间件配置
\bin 编译后的目录
\Middlerware  中间件目录，在管道请求中配置使用，使得每个请求都必须经过该中间件处理之后才能进入下一个委托
\Services 服务目录，存放身份验证服务实现


[我的博客](https://blog.csdn.net/cslx5zx5)  