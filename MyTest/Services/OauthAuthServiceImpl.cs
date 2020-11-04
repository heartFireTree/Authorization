using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyTest.Config;
using MyTest.Entity;
using MyTest.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTest.Services
{
    public class OauthAuthServiceImpl : IAuthService
    {
        private readonly AuthDbContext _passportDbContext;
        private readonly GlobalSettings _globalSettings;
        private readonly LoginUser _currentUser;

        public OauthAuthServiceImpl(AuthDbContext passportDbContext, IOptions<GlobalSettings> options, LoginUser currentUser)
        {
            _passportDbContext = passportDbContext;
            _globalSettings = options.Value;
            _currentUser = currentUser;
        }

        public string ServiceName => nameof(OauthAuthServiceImpl);

        /// <summary>
        /// 测试TOKEN
        /// </summary>
        public static string TEST_TOKEN = "73c8aa709d744848b3f4b697b48905ca";
        /// <summary>
        /// 测试用户
        /// </summary>
        public static string TEST_ADMIN = "a437a67c2de345a9a5c56ade745c0ecd";

        public async Task<bool> PermissionAsync(string token, string path)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (token == TEST_TOKEN)
            {
                await SetUserAsync(token);
                return true;
            }
            //Token过期
            var tokenEntity = await _passportDbContext.SysUserTokens.Where(s => s.Token == token).FirstOrDefaultAsync();
            if (tokenEntity == null) return false;
            if (tokenEntity.ExpireTime != null && tokenEntity.ExpireTime <= DateTime.Now) return false;

            return true;
        }

        public async Task SetUserAsync(string token)
        {
            if (string.IsNullOrEmpty(_currentUser.UserId))
            {
                //TODO: 固定一个测试用户Token
                if (token == TEST_TOKEN)
                {
                    var user = new LoginUser
                    {
                        UserId = Guid.NewGuid().ToString("N"),
                        Mobile = "13012345678",
                        UserName = "Roy",
                        TenantId = DateTime.Now.ToString()
                    };
                    user.CopyTo(_currentUser);
                }
                else
                {
                    var user = await (from t in _passportDbContext.SysUserTokens.AsNoTracking()
                                      join u in _passportDbContext.Users.AsNoTracking()
                                      on t.UserId equals u.UserId
                                      where t.Token == token
                                      select new LoginUser
                                      {
                                          UserId = u.UserId,
                                          Mobile = u.Mobile,
                                          UserName = u.Username,
                                          TenantId = u.TenantId
                                      }).FirstOrDefaultAsync();
                    user?.CopyTo(_currentUser);
                }
            }
        }
    }
}
