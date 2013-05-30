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

using System.Linq;
using System.Net;
using License.Manager.Core.Model;
using License.Manager.Core.Persistence;
using License.Manager.Core.ServiceModel;
using Raven.Client;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;

namespace License.Manager.Core.ServiceInterface
{
    [Authenticate]
    public class CustomerService : Service
    {
        private readonly IDocumentSession documentSession;

        public CustomerService(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public object Post(Customer customer)
        {
            documentSession.Store(customer);
            documentSession.SaveChanges();

            return
                new HttpResult(customer)
                    {
                        StatusCode = HttpStatusCode.Created,
                        Headers =
                            {
                                {HttpHeaders.Location, Request.AbsoluteUri.CombineWith(customer.Id)}
                            }
                    };
        }

        public object Put(Customer customer)
        {
            documentSession.Store(customer);
            documentSession.SaveChanges();

            return customer;
        }

        public object Delete(Customer customer)
        {
            documentSession.Delete(documentSession.Load<Customer>(customer.Id));
            documentSession.SaveChanges();

            return
                new HttpResult
                    {
                        StatusCode = HttpStatusCode.NoContent,
                    };
        }

        public object Get(Customer customer)
        {
            return documentSession.Load<Customer>(customer.Id);
        }

        public object Get(FindCustomers request)
        {
            var query = documentSession.Query<CustomerAllPropertiesIndex.Result, CustomerAllPropertiesIndex>();

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Search(c => c.Query, request.Name);

            if (!string.IsNullOrWhiteSpace(request.Company))
                query = query.Search(c => c.Query, request.Company);

            if (!string.IsNullOrWhiteSpace(request.Email))
                query = query.Search(c => c.Query, request.Email);

            return query.OfType<Customer>().ToList();
        }
    }
}