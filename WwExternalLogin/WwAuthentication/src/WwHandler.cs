using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Ww External Login Hanlder
    /// </summary>
    /// <see cref="https://work.weixin.qq.com/api/doc/90000/90135/90988"/>
    public partial class WwHandler : OAuthHandler<WwOptions>
    {
        private JsonSerializerOptions _serializerOpts = null;

        public WwHandler(IOptionsMonitor<WwOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _serializerOpts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            _serializerOpts.Converters.Add(new JsonStringEnumConverter());
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var state = Options.StateDataFormat.Protect(properties);
            var endpoint = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, "appid", Options.ClientId);

            // 拼接微信扫码的地址，如：https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid=CORPID&agentid=AGENTID&redirect_uri=REDIRECT_URI&state=STATE

            endpoint = QueryHelpers.AddQueryString(endpoint, "agentid", Options.AgentId);
            endpoint = QueryHelpers.AddQueryString(endpoint, "state", state);
            endpoint = QueryHelpers.AddQueryString(endpoint, "redirect_uri", redirectUri);

            return endpoint;
        }
        
        /// <summary>
        /// 获取微信的 access token，实际上不需要使用 Code
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            string endpoint = QueryHelpers.AddQueryString(Options.TokenEndpoint, "corpid", Options.ClientId);
            endpoint = QueryHelpers.AddQueryString(endpoint, "corpsecret", Options.ClientSecret);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Version = Backchannel.DefaultRequestVersion;
            var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var respObj = JsonSerializer.Deserialize<WxAccessTokenResponse>(await response.Content.ReadAsStringAsync(), _serializerOpts);
                if (respObj.Errcode == 0)
                {
                    respObj.Code = context.Code;
                    return OAuthTokenResponse.Success(JsonDocument.Parse(JsonSerializer.Serialize(respObj, _serializerOpts)));
                }
            }

            var error = "OAuth token endpoint failure: ";
            return OAuthTokenResponse.Failed(new Exception(error));
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            JsonDocument payload = null;

            string userinfoEndpoint = QueryHelpers.AddQueryString(Options.UserInformationEndpoint.TrimEnd('/') + "/getuserinfo", "access_token", tokens.AccessToken);
            string code = tokens.Response.RootElement.GetString("code");

            userinfoEndpoint = QueryHelpers.AddQueryString(userinfoEndpoint, "code", code);

            // 获取微信企业版用户信息
            var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, userinfoEndpoint);

            var userInfoResponse = await Backchannel.SendAsync(userInfoRequest, Context.RequestAborted);
            if (!userInfoResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occurred when retrieving Dingtalk user information ({userInfoResponse.StatusCode}).");
            }

            string rawResp = await userInfoResponse.Content.ReadAsStringAsync();
            var wwUserInfo = JsonSerializer.Deserialize<WwUserInfoDto>(rawResp, _serializerOpts);
            if (wwUserInfo.Errcode != 0)
            {
                throw new HttpRequestException($"An error occurred when retrieving Dingtalk user information ({wwUserInfo.Errmsg}).");
            }

            // 获取成员信息
            string userEndpoint = QueryHelpers.AddQueryString(Options.UserInformationEndpoint.TrimEnd('/') + "/get", "access_token", tokens.AccessToken);
            userEndpoint = QueryHelpers.AddQueryString(userEndpoint, "userid", wwUserInfo.UserId);
            var request = new HttpRequestMessage(HttpMethod.Get, userEndpoint);

            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occurred when retrieving Dingtalk user information ({userInfoResponse.StatusCode}).");
            }

            string rawPayload = await response.Content.ReadAsStringAsync();
            var wwUser = JsonSerializer.Deserialize<WwUserDto>(rawPayload, _serializerOpts);
            if (wwUser.Errcode != 0)
            {
                throw new HttpRequestException($"An error occurred when retrieving Dingtalk user information ({wwUserInfo.Errmsg}).");
            }

            payload = JsonDocument.Parse(rawPayload);

            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
            context.RunClaimActions();

            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }
    }

}
