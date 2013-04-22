using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.Model
{
    [Route("/products/{Id}", "GET, OPTIONS")]
    [Route("/products/{Id}", "PUT, DELETE")]
    public class Product : EntityBase, IReturn<Product>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public KeyPair KeyPair { get; set; }

        public List<string> ProductFeatures { get; set; }
    }
}