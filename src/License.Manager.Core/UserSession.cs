//
// Copyright © 2012 - 2013 Nauck IT KG     http://www.nauck-it.de
//
// Author:
//  Daniel Nauck        <d.nauck(at)nauck-it.de>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using ServiceStack.Authentication.OpenId;
using ServiceStack.Common;
using ServiceStack.ServiceInterface.Auth;

namespace License.Manager.Core
{
    public class UserSession : AuthUserSession
    {
        public override void OnAuthenticated(ServiceStack.ServiceInterface.IServiceBase authService, IAuthSession session, IOAuthTokens tokens, System.Collections.Generic.Dictionary<string, string> authInfo)
        {
            base.OnAuthenticated(authService, session, tokens, authInfo);

            //Populate all matching fields from this session to your own custom User table

            foreach (var authToken in session.ProviderOAuthAccess)
            {
                if (authToken.Provider == GoogleOpenIdOAuthProvider.Name)
                {
                    //user.GoogleUserId = authToken.UserId;
                    //user.GoogleFullName = authToken.FullName;
                    //user.GoogleEmail = authToken.Email;
                }
                else if (authToken.Provider == YahooOpenIdOAuthProvider.Name)
                {
                    //user.YahooUserId = authToken.UserId;
                    //user.YahooFullName = authToken.FullName;
                    //user.YahooEmail = authToken.Email;
                }
            }
        }
    }
}