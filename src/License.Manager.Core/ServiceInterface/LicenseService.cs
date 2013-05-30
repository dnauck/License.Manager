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

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Configuration;
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
            return Enum.GetValues(typeof(Portable.Licensing.LicenseType))
                       .Cast<Portable.Licensing.LicenseType>().ToList();
        }

        public object Post(IssueLicense issueRequest)
        {
            var machineKeySection = WebConfigurationManager.GetSection("system.web/machineKey") as MachineKeySection;
            if (machineKeySection == null || StringComparer.OrdinalIgnoreCase.Compare(machineKeySection.Decryption, "Auto") == 0)
                throw new Exception(Properties.Resources.InvalidMachineKeySection);

            var license = documentSession.Load<Model.License>(issueRequest.Id);

            var licenseFile =
                Portable.Licensing.License.New()
                        .WithUniqueIdentifier(license.LicenseId)
                        .As(license.LicenseType)
                        .WithMaximumUtilization(license.Quantity)
                        .ExpiresAt(license.Expiration)
                        .LicensedTo(customer =>
                                        {
                                            customer.Name = license.Customer.Name;
                                            customer.Email = license.Customer.Email;
                                            customer.Company = license.Customer.Company;
                                        })
                        .WithProductFeatures(license.ProductFeatures)
                        .WithAdditionalAttributes(license.AdditionalAttributes)
                        .CreateAndSignWithPrivateKey(license.Product.KeyPair.EncryptedPrivateKey,
                                                     machineKeySection.DecryptionKey);

            var issueToken = Guid.NewGuid().ToString();
            cacheClient.Set(UrnId.Create<Model.License>("IssueToken", issueToken), licenseFile, new TimeSpan(0, 5, 0));

            return new HttpResult
                       {
                           StatusCode = HttpStatusCode.Created,
                           Headers =
                               {
                                   {HttpHeaders.Location, Request.AbsoluteUri.AddQueryParam("token", issueToken)}
                               }
                       };
        }

        public object Get(DownloadLicense downloadRequest)
        {
            var cacheKey = UrnId.Create<Model.License>("IssueToken", downloadRequest.Token.ToString());
            var license = cacheClient.Get<Portable.Licensing.License>(cacheKey);

            if (license == null)
                return new HttpResult(HttpStatusCode.NotFound);

            var responseStream = new MemoryStream();
            license.Save(responseStream);

            var response = new HttpResult(responseStream, "application/xml");
            response.Headers[HttpHeaders.ContentDisposition] = "attachment; filename=License.lic";

            return response;
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