using System;
using System.Collections.Generic;
using Portable.Licensing;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/licenses", "POST, OPTIONS")]
    [Route("/products/{ProductId}/licenses", "POST, OPTIONS")]
    [Route("/customers/{CustomerId}/licenses", "POST, OPTIONS")]
    public class CreateLicense : IReturn<Model.License>
    {
        public Guid LicenseId { get; set; }
        public LicenseType LicenseType { get; set; }
        public int Quantity { get; set; }
        public DateTime Expiration { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public Dictionary<string, string> ProductFeatures { get; set; }
        public Dictionary<string, string> AdditionalAttributes { get; set; }
    }
}