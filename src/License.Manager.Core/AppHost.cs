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

using Funq;
using Raven.Client;
using Raven.Client.Indexes;
using ServiceStack.Authentication.OpenId;
using ServiceStack.Authentication.RavenDb;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Configuration;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.Cors;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Extensions;

namespace License.Manager.Core
{
    public class AppHost : AppHostBase
    {
        public AppHost() //Tell ServiceStack the name and where to find your web services
            : base("License.Manager ASP.NET Host", typeof(AppHost).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            SetConfig(new EndpointHostConfig {ServiceStackHandlerFactoryPath = "api"});

            //Set JSON web services to return idiomatic JSON camelCase properties
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
            ServiceStack.Text.JsConfig.DateHandler = JsonDateHandler.ISO8601;

            Plugins.Add(new CorsFeature()); //Registers global CORS Headers

            RequestFilters.Add((httpReq, httpRes, requestDto) =>
                                   {
                                       //Handles Request and closes Responses after emitting global HTTP Headers
                                       if (httpReq.HttpMethod == "OPTIONS")
                                           httpRes.EndServiceStackRequest();
                                   });

            //Enable the validation feature
            Plugins.Add(new ValidationFeature());

            //This method scans the assembly for validators
            container.RegisterValidators(typeof(AppHost).Assembly);

            container.Register<ICacheClient>(new MemoryCacheClient());

            // register RavenDB dependencies
            ConfigureRavenDb(container);

            // register authentication framework
            ConfigureAuthentication(container);
        }

        public void ConfigureRavenDb(Container container)
        {
            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);

            var documentStore =
                new Raven.Client.Embedded.EmbeddableDocumentStore()
                    {
                        UseEmbeddedHttpServer = true
                    }
                    .Initialize();


            IndexCreation.CreateIndexes(GetType().Assembly, documentStore);

            container.Register(documentStore);

            container.Register(c => c.Resolve<IDocumentStore>().OpenSession())
                     .ReusedWithin(ReuseScope.Request);

        }

        private void ConfigureAuthentication(Container container)
        {
            var appSettings = new AppSettings();

            //Default route: /auth/{provider}
            Plugins.Add(new AuthFeature(() => new UserSession(),
                                        new IAuthProvider[]
                                            {
                                                new CredentialsAuthProvider(appSettings),
                                                //new TwitterAuthProvider(appSettings), //Sign-in with Twitter
                                                //new FacebookAuthProvider(appSettings), //Sign-in with Facebook
                                                //new DigestAuthProvider(appSettings), //Sign-in with Digest Auth
                                                //new BasicAuthProvider(), //Sign-in with Basic Auth

                                                //Register new OpenId providers you want to allow authentication with
                                                new GoogleOpenIdOAuthProvider(appSettings), //Sign-in with Goolge OpenId
                                                new YahooOpenIdOAuthProvider(appSettings), //Sign-in with Yahoo OpenId
                                                new MyOpenIdOAuthProvider(appSettings), //Sign-in with MyOpenId
                                                new OpenIdOAuthProvider(appSettings),  //Sign-in with any Custom OpenId Provider
                                            }));

            container.Register(c => new RavenUserAuthRepository(c.Resolve<IDocumentStore>()));
            container.Register<IUserAuthRepository>(c => c.Resolve<RavenUserAuthRepository>());
        }
    }
}