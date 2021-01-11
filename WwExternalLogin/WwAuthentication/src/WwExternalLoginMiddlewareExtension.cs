using System;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 微信企业版外部登录扩展
    /// </summary>
    public static class WwExternalLoginMiddlewareExtension
    {
        /// <summary>
        /// 提供微信企业版外部登陆
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddWw(this AuthenticationBuilder builder, Action<WwOptions> configureOptions) =>
            builder.AddOAuth<WwOptions, WwHandler>("Ww", "Ww", configureOptions);

        /// <summary>
        /// 提供微信企业版外部登陆
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="displayName">显示的名称</param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddWw(this AuthenticationBuilder builder, string displayName, Action<WwOptions> configureOptions) =>
            builder.AddOAuth<WwOptions, WwHandler>("Ww", displayName, configureOptions);
    }
}
