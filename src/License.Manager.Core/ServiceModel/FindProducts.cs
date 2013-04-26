using System.Collections.Generic;
using License.Manager.Core.Model;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/products", "GET")]
    //[Route("/products/page/{Page}")]
    public class FindProducts : IReturn<List<Product>>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        //public int? Page { get; set; }
        //public int? PageSize { get; set; }
    }
}