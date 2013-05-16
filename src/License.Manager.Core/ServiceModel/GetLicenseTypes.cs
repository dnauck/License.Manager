using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/licenses/types", "GET, OPTIONS")]
    public class GetLicenseTypes : IReturn<List<Portable.Licensing.LicenseType>>
    {
    }
}
