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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using License.Manager.Core.Model;
using License.Manager.Core.Persistence;
using License.Manager.Core.ServiceModel;
using Raven.Client;
using ServiceStack.Authentication.OpenId;
using ServiceStack.Authentication.RavenDb;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace License.Manager.Core.ServiceInterface
{
    [Authenticate]
    public class AccountService : Service
    {
        private readonly IDocumentSession documentSession;
        private readonly RavenUserAuthRepository userAuthRepository;

        public AccountService(IDocumentSession documentSession, RavenUserAuthRepository userAuthRepository)
        {
            this.documentSession = documentSession;
            this.userAuthRepository = userAuthRepository;
        }

        //public object Put(UpdateCustomer request)
        //{
        //    var customer = documentSession.Load<Customer>(request.Id);
        //    if (customer == null)
        //        HttpError.NotFound("Customer not found!");

        //    customer.PopulateWith(request);

        //    documentSession.Store(customer);
        //    documentSession.SaveChanges();

        //    return customer;
        //}

        [Route("/accounts/oauthproviders", "GET, OPTIONS")]
        public class GetOAuthProviders : IReturn<List<OAuthProvider>>
        {
        }

        public class OAuthProvider
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
        }

        public object Get(GetOAuthProviders request)
        {
            var result = new List<OAuthProvider>
                             {
                                 new OAuthProvider()
                                     {
                                         Name = GoogleOpenIdOAuthProvider.Name,
                                         DisplayName = "Google"
                                     },
                                 new OAuthProvider()
                                     {
                                         Name = YahooOpenIdOAuthProvider.Name,
                                         DisplayName = "Yahoo"
                                     },
                                 new OAuthProvider()
                                     {
                                         Name = MyOpenIdOAuthProvider.Name,
                                         DisplayName = "MyOpenId"
                                     },
                                 new OAuthProvider()
                                     {
                                         Name = OpenIdOAuthProvider.DefaultName,
                                         DisplayName = "OpenId"
                                     }
                             };

            return result;
        }

        public object Get(GetAccount request)
        {
            if (request.Id.HasValue)
            {
                var accountId = string.Concat("UserAuths/", request.Id.Value.ToString(CultureInfo.InvariantCulture));
                var account = userAuthRepository.GetUserAuth(string.Concat("UserAuths/", request.Id.Value.ToString(CultureInfo.InvariantCulture)));
                if (account == null)
                    HttpError.NotFound("Account not found!");

                var providers = Get((GetOAuthProviders) null) as List<OAuthProvider>;
                var activeProviders = userAuthRepository.GetUserOAuthProviders(account.Id.ToString());
                //var a2 = activeProviders.SelectMany(provider => )
                                  //.Select(x => providers.First(f => f.Name == x.Provider));
                
                return new AccountDto().PopulateWith(account);
            }

            return new AccountDto().PopulateWith(SessionAs<UserSession>());
        }

        public object Get(FindAccounts request)
        {
            //var query = documentSession.Query<CustomerAllPropertiesIndex.Result, CustomerAllPropertiesIndex>();

            //if (!string.IsNullOrWhiteSpace(request.Name))
            //    query = query.Search(c => c.Query, request.Name);

            //if (!string.IsNullOrWhiteSpace(request.Email))
            //    query = query.Search(c => c.Query, request.Email);

            var query = documentSession.Query<UserAuth>();

            return query
                .OfType<UserAuth>()
                .ToList();
        }
    }
}