using ServiceStack.ServiceHost;

namespace License.Manager.Core.Model
{
    [Route("/customers", "POST, OPTIONS")]
    [Route("/customers/{Id}", "GET, PUT, DELETE, OPTIONS")]
    public class Customer : EntityBase, IReturn<Customer>
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
    }
}