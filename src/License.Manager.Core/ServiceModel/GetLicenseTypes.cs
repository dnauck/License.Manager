using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/licenses/types", "GET, OPTIONS")]
    public class GetLicenseTypes : IReturn<List<Portable.Licensing.LicenseType>>
    {
    }
}
