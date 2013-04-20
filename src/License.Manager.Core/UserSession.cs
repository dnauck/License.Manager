using ServiceStack.ServiceInterface.Auth;

namespace License.Manager.Core
{
    public class UserSession : AuthUserSession
    {
        public override void OnAuthenticated(ServiceStack.ServiceInterface.IServiceBase authService, IAuthSession session, IOAuthTokens tokens, System.Collections.Generic.Dictionary<string, string> authInfo)
        {
            base.OnAuthenticated(authService, session, tokens, authInfo);
        }
    }
}