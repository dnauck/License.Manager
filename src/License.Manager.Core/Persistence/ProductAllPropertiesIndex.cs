using System.Linq;
using License.Manager.Core.Model;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace License.Manager.Persistence
{
    public class ProductAllPropertiesIndex : AbstractIndexCreationTask<Product, ProductAllPropertiesIndex.Result>
    {
        public class Result
        {
            public string Query { get; set; }
        }

        public ProductAllPropertiesIndex()
        {
            Map = products =>
                  from product in products
                  select new
                             {
                                 Query = AsDocument(product).Select(x => x.Value)
                             };

            Index(x => x.Query, FieldIndexing.Analyzed);
        }
    }
}