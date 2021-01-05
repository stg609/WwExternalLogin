using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 微信企业版 外部登录选项
    /// </summary>
    public class WwOptions : OAuthOptions
    {
        public WwOptions()
        {
            CallbackPath = new PathString("/signin-ww");
            AuthorizationEndpoint = "https://open.work.weixin.qq.com/wwopen/sso/qrConnect";
            TokenEndpoint = "https://qyapi.weixin.qq.com/cgi-bin/gettoken";
            UserInformationEndpoint = "https://qyapi.weixin.qq.com/cgi-bin/user/";

            // 必须，用于 Asp.net core identity GetExternalLoginInfoAsync 判断用户是否存在
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "userid");

            // 额外的标准的 claim
            ClaimActions.MapJsonKey("name", "name");
            ClaimActions.MapJsonKey("sub", "userid");
            ClaimActions.MapJsonKey("phone_number", "mobile");
            ClaimActions.MapJsonKey("email", "email");
            ClaimActions.MapJsonKey("picture", "avatar");

            // 额外的非标准 claim
            ClaimActions.MapJsonKey(WwClaimTypes.Position, "position");
        }

        /// <summary>
        /// 授权方的网页应用ID
        /// </summary>
        public string AgentId { get; set; }

    }

}
