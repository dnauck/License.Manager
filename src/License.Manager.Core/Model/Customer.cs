using ServiceStack.ServiceHost;

namespace License.Manager.Core.Model
{
    [Route("/customers", "POST")]
    [Route("/customers/{Id}", "PUT, DELETE")]
    [Route("/customers/{Id}", "GET, OPTIONS")]
    public class Customer : EntityBase
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
    }
}