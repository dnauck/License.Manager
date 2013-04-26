using License.Manager.Core.ServiceModel;
using ServiceStack.FluentValidation;

namespace License.Manager.Core.Validation
{
    public class CreateProductValidator : AbstractValidator<CreateProduct>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.PrivateKeyPassPhrase).NotEmpty();
        }
    }
}