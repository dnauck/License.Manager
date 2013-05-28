using System;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/licenses/issue", "POST, OPTIONS")]
    [Route("/licenses/{Id}/issue", "POST, OPTIONS")]
    [Route("/products/{ProductId}/licenses/issue", "POST, OPTIONS")]
    [Route("/products/{ProductId}/licenses/{Id}/issue", "POST, OPTIONS")]
    [Route("/customers/{CustomerId}/licenses/issue", "POST, OPTIONS")]
    [Route("/customers/{CustomerId}/licenses/{Id}/issue", "POST, OPTIONS")]
    public class IssueLicense
    {
        public int Id { get; set; }
        public Guid LicenseId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
    }
}