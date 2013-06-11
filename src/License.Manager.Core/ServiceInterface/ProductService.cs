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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using License.Manager.Core.Model;
using License.Manager.Core.Persistence;
using License.Manager.Core.ServiceModel;
using Portable.Licensing.Security.Cryptography;
using Raven.Client;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using KeyPair = License.Manager.Core.Model.KeyPair;

namespace License.Manager.Core.ServiceInterface
{
    [Authenticate]
    public class ProductService : Service
    {
        private readonly IDocumentSession documentSession;

        public ProductService(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public object Post(CreateProduct request)
        {
            var machineKeySection = WebConfigurationManager.GetSection("system.web/machineKey") as MachineKeySection;
            if (machineKeySection == null ||
                StringComparer.OrdinalIgnoreCase.Compare(machineKeySection.Decryption, "Auto") == 0)
                throw new Exception(Properties.Resources.InvalidMachineKeySection);

            var product =
                new Product
                    {
                        KeyPair = GenerateKeyPair(machineKeySection.DecryptionKey)
                    }.PopulateWith(request);

            documentSession.Store(product);
            documentSession.SaveChanges();

            return
                new HttpResult(new ProductDto().PopulateWith(product))
                    {
                        StatusCode = HttpStatusCode.Created,
                        Headers =
                            {
                                {HttpHeaders.Location, Request.AbsoluteUri.CombineWith(product.Id)}
                            }
                    };
        }

        private static KeyPair GenerateKeyPair(string privateKeyPassPhrase)
        {
            var keyGenerator = KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();

            var result =
                new KeyPair
                    {
                        PublicKey = keyPair.ToPublicKeyString(),
                        EncryptedPrivateKey = keyPair.ToEncryptedPrivateKeyString(privateKeyPassPhrase)
                    };

            return result;
        }

        public object Put(UpdateProduct request)
        {
            var product = documentSession.Load<Product>(request.Id);
            if (product == null)
                HttpError.NotFound("Product not found!");

            product.PopulateWith(request);

            documentSession.Store(product);
            documentSession.SaveChanges();

            return new ProductDto().PopulateWith(product);
        }

        public object Delete(UpdateProduct request)
        {
            var product = documentSession.Load<Product>(request.Id);
            if (product == null)
                HttpError.NotFound("Product not found!");

            documentSession.Delete(product);
            documentSession.SaveChanges();

            return
                new HttpResult
                    {
                        StatusCode = HttpStatusCode.NoContent,
                    };
        }

        public object Get(GetProduct request)
        {
            var product = documentSession.Load<Product>(request.Id);
            if (product == null)
                HttpError.NotFound("Product not found!");

            return new ProductDto().PopulateWith(product);
        }

        public object Get(FindProducts request)
        {
            var query = documentSession.Query<Product_ByNameOrDescription.Result, Product_ByNameOrDescription>();

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Search(c => c.Name, request.Name);

            if (!string.IsNullOrWhiteSpace(request.Description))
                query = query.Search(c => c.Name, request.Description);

            var products = query
                .OfType<Product>()
                .ToList();

            var result = new List<ProductDto>(products.Count);
            result.AddRange(
                products.Select(
                    product => new ProductDto().PopulateWith(product)));

            return result;
        }
    }
}