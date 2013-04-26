using System.Linq;
using System.Net;
using License.Manager.Core.Model;
using License.Manager.Core.ServiceModel;
using License.Manager.Persistence;
using Portable.Licensing.Security.Cryptography;
using Raven.Client;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using KeyPair = License.Manager.Core.Model.KeyPair;

namespace License.Manager.Core.ServiceInterface
{
    [Authenticate]
    public class ProductService : Service
    {
        private readonly IDocumentSession documentSession;

        public ProductService(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public object Post(CreateProduct newProduct)
        {
            var product =
                new Product
                    {
                        Name = newProduct.Name,
                        Description = newProduct.Description,
                        ProductFeatures = newProduct.ProductFeatures,
                        KeyPair = GenerateKeyPair(newProduct.PrivateKeyPassPhrase)
                    };

            documentSession.Store(product);
            documentSession.SaveChanges();

            return
                new HttpResult(product)
                    {
                        StatusCode = HttpStatusCode.Created,
                        Headers =
                            {
                                {HttpHeaders.Location, Request.AbsoluteUri.CombineWith(product.Id)}
                            }
                    };
        }

        private static KeyPair GenerateKeyPair(string privateKeyPassPhrase)
        {
            var keyGenerator = KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();

            var result =
                new KeyPair
                    {
                        PublicKey = keyPair.ToPublicKeyString(),
                        EncryptedPrivateKey = keyPair.ToEncryptedPrivateKeyString(privateKeyPassPhrase)
                    };

            return result;
        }

        public object Put(Product product)
        {
            documentSession.Store(product);
            documentSession.SaveChanges();

            return product;
        }

        public object Delete(Product product)
        {
            documentSession.Delete(documentSession.Load<Product>(product.Id));
            documentSession.SaveChanges();

            return
                new HttpResult
                    {
                        StatusCode = HttpStatusCode.NoContent,
                    };
        }

        public object Get(Product product)
        {
            return documentSession.Load<Product>(product.Id);
        }

        public object Get(FindProducts request)
        {
            var query = documentSession.Query<ProductAllPropertiesIndex.Result, ProductAllPropertiesIndex>();

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Search(c => c.Query, request.Name);

            if (!string.IsNullOrWhiteSpace(request.Description))
                query = query.Search(c => c.Query, request.Description);

            return query.OfType<Product>().ToList();
        }
    }
}