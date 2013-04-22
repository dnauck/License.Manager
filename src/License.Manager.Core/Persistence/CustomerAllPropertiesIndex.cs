using System.Linq;
using License.Manager.Core.Model;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace License.Manager.Core.Persistence
{
    public class CustomerAllPropertiesIndex : AbstractIndexCreationTask<Customer, CustomerAllPropertiesIndex.Result>
    {
        public class Result
        {
            public string Query { get; set; }
        }

        public CustomerAllPropertiesIndex()
        {
            Map = customers =>
                  from customer in customers
                  select new
                             {
                                 Query = AsDocument(customer).Select(x => x.Value)
                             };

            Index(x => x.Query, FieldIndexing.Analyzed);
        }
    }
}