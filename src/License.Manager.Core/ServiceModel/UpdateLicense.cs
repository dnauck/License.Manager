using System;
using System.Collections.Generic;
using Portable.Licensing;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/licenses/{Id}", "PUT, DELETE, OPTIONS")]
    [Route("/products/{ProductId}/licenses/{Id}", "PUT, DELETE, OPTIONS")]
    [Route("/customers/{CustomerId}/licenses/{Id}", "PUT, DELETE, OPTIONS")]
    public class UpdateLicense : IReturn<Model.License>
    {
        public int Id { get; set; }
        public LicenseType LicenseType { get; set; }
        public int Quantity { get; set; }
        public DateTime Expiration { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public Dictionary<string, string> ProductFeatures { get; set; }
        public Dictionary<string, string> AdditionalAttributes { get; set; }
    }
}