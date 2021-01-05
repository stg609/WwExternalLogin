namespace Microsoft.Extensions.DependencyInjection
{
    public partial class WwHandler
    {
        public class WxAccessTokenResponse
        {
            public int Errcode { get; set; }
            public string Errmsg { get; set; }
            public string Access_token { get; set; }
            public int Expires_in { get; set; }

            public string Code { get; set; }
        }
    }

}
