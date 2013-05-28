using System.Collections.Generic;
using License.Manager.Core.Model;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/products", "POST")]
    public class CreateProduct : IReturn<Product>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductFeatures { get; set; }
    }
}