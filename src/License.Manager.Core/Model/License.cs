using System;
using System.Collections.Generic;
using Portable.Licensing;

namespace License.Manager.Core.Model
{
    public class License : EntityBase
    {
        public Guid LicenseId { get; set; }
        public LicenseType LicenseType { get; set; }
        public int Quantity { get; set; }
        public DateTime Expiration { get; set; }
        public Customer Customer { get; set; }
        public Product Product { get; set; }
        public Dictionary<string, string> ProductFeatures { get; set; }
        public Dictionary<string, string> AdditionalAttributes { get; set; }
    }
}