//
// Copyright © 2012 - 2013 Nauck IT KG     http://www.nauck-it.de
//
// Author:
//  Daniel Nauck        <d.nauck(at)nauck-it.de>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Portable.Licensing;
using ServiceStack.ServiceHost;

namespace License.Manager.Core.ServiceModel
{
    [Route("/licenses/{Id}", "PUT, DELETE, OPTIONS")]
    [Route("/products/{ProductId}/licenses/{Id}", "PUT, DELETE, OPTIONS")]
    [Route("/customers/{CustomerId}/licenses/{Id}", "PUT, DELETE, OPTIONS")]
    public class UpdateLicense : IReturn<LicenseDto>
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