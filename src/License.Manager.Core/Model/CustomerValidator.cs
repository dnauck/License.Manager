using ServiceStack.FluentValidation;

namespace License.Manager.Core.Model
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Email).EmailAddress().NotEmpty();
        }
    }
}