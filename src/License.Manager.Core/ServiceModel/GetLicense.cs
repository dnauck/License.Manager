using System;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/products/{ProductId}/licenses/{Id}", "GET, OPTIONS")]
    [Route("/products/{ProductId}/licenses/{LicenseId}", "GET, OPTIONS")]
    [Route("/customers/{CustomerId}/licenses/{Id}", "GET, OPTIONS")]
    [Route("/customers/{CustomerId}/licenses/{LicenseId}", "GET, OPTIONS")]
    public class GetLicense : IReturn<Model.License>
    {
        public int Id { get; set; }
        public Guid LicenseId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
    }
}