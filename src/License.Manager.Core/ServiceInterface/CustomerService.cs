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