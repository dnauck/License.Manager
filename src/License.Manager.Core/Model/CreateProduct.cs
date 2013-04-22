using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.Model
{
    [Route("/products", "POST")]
    public class CreateProduct : IReturn<Product>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string PrivateKeyPassPhrase { get; set; }

        public List<string> ProductFeatures { get; set; }
    }
}