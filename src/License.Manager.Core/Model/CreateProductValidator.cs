using ServiceStack.FluentValidation;

namespace License.Manager.Core.Model
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