using System;
using System.Linq;
using System.Net;
using License.Manager.Core.Model;
using License.Manager.Core.ServiceModel;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Linq;
using ServiceStack.CacheAccess;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;

namespace License.Manager.Core.ServiceInterface
{
    [Authenticate]
    public class LicenseService : Service
    {
        private readonly IDocumentSession documentSession;
        private readonly ICacheClient cacheClient;

        public LicenseService(IDocumentSession documentSession, ICacheClient cacheClient)
        {
            this.documentSession = documentSession;
            this.cacheClient = cacheClient;
        }

        public object Get(GetLicenseTypes request)
        {
            return Enum.GetValues(typeof (Portable.Licensing.LicenseType))
                       .Cast<Portable.Licensing.LicenseType>().ToList();
        }

        public object Post(CreateLicense license)
        {
            var newLicense = new Model.License().PopulateWith(license);
            newLicense.Product = documentSession.Load<Product>(license.ProductId);
            newLicense.Customer = documentSession.Load<Customer>(license.CustomerId);

            documentSession.Store(newLicense);
            documentSession.SaveChanges();

            return
                new HttpResult(HideProductKeyInformation(newLicense))
                    {
                        StatusCode = HttpStatusCode.Created,
                        Headers =
                            {
                                {HttpHeaders.Location, Request.AbsoluteUri.CombineWith(newLicense.Id)}
                            }
                    };
        }

        public object Put(UpdateLicense license)
        {
            var updateLicense = documentSession
                .Load<Model.License>(license.Id)
                .PopulateWith(license);

            documentSession.Store(updateLicense);
            documentSession.SaveChanges();

            return HideProductKeyInformation(updateLicense);
        }

        public object Delete(UpdateLicense license)
        {
            documentSession.Delete(documentSession.Load<Model.License>(license.Id));
            documentSession.SaveChanges();

            return
                new HttpResult
                    {
                        StatusCode = HttpStatusCode.NoContent,
                    };
        }

        public object Get(GetLicense license)
        {
            return HideProductKeyInformation(documentSession.Load<Model.License>(license.Id));
        }

        public object Get(FindLicenses request)
        {
            var query = documentSession.Query<Model.License>();

            if (request.LicenseType > 0)
                query = query.Where(lic => lic.LicenseType.Is(request.LicenseType));

            if (request.CustomerId > 0)
                query = query.Where(lic => lic.Customer.Id == request.CustomerId);

            if (request.ProductId > 0)
                query = query.Where(lic => lic.Product.Id == request.ProductId);

            query.ForEach(license => HideProductKeyInformation(license));
            return query.OfType<Model.License>().ToList();
        }

        private static Model.License HideProductKeyInformation(Model.License license)
        {
            if (license.Product != null)
                license.Product.KeyPair = null;

            return license;
        }
    }
}