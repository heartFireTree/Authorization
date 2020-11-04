using Microsoft.Extensions.Options;
using MyTest.Config;
using MyTest.Entity;
using MyTest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTest.Services
{
    public class JwtAuthServiceImpl : IAuthService
    {
        private readonly GlobalSettings _globalSettings;
        private readonly LoginUser _currentUser;

        public JwtAuthServiceImpl(IOptions<GlobalSettings> options, LoginUser currentUser)
        {
            _globalSettings = options.Value;
            
            _currentUser = currentUser;
        }

        public string ServiceName => nameof(JwtAuthServiceImpl);

        public Task<bool> PermissionAsync(string token, string path)
        {
            return Task.FromResult(JwtUtils.CheckToken(token, _globalSettings.Jwt.Secret));
        }

        public Task SetUserAsync(string token)
        {
            var payload = JwtUtils.GetPayload(token);
            _currentUser.UserId = payload.sub;
            return Task.CompletedTask;
        }
    }
}
