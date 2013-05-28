using Funq;
using Raven.Client;
using Raven.Client.Indexes;
using ServiceStack.Authentication.RavenDb;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Configuration;
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
                                                //new FacebookAuthProvider(appSettings),
                                                //new TwitterAuthProvider(appSettings),
                                                //new BasicAuthProvider(appSettings),
                                            }));

            //Default route: /register
            Plugins.Add(new RegistrationFeature());

            container.Register<IUserAuthRepository>(c =>
                                                    new RavenUserAuthRepository(c.Resolve<IDocumentStore>()));
        }
    }
}