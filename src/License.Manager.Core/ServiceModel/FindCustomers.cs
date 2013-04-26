using System.Collections.Generic;
using License.Manager.Core.Model;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/customers", "GET")]
    //[Route("/customers/page/{Page}")]
    //[Route("/products/{Id}/customers", "GET")]
    public class FindCustomers : IReturn<List<Customer>>
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }

        //public int? Page { get; set; }
        //public int? PageSize { get; set; }
    }
}