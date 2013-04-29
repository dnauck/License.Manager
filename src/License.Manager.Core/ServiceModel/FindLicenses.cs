using System;
using System.Collections.Generic;
using Portable.Licensing;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/licenses", "GET, OPTIONS")]
    [Route("/products/{ProductId}/licenses", "GET, OPTIONS")]
    [Route("/customers/{CustomerId}/licenses", "GET, OPTIONS")]
    //[Route("/licenses/page/{Page}")]
    public class FindLicenses : IReturn<List<Model.License>>
    {
        public LicenseType LicenseType { get; set; }
        public DateTime Expiration { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        //public int? Page { get; set; }
        //public int? PageSize { get; set; }
    }
}