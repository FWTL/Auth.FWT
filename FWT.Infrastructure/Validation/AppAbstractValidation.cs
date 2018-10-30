using FluentValidation;
using FluentValidation.Results;

namespace FWT.Infrastructure.Validation
{
    public abstract class AppAbstractValidation<TModel> : AbstractValidator<TModel>
    {
        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
        }

        public override ValidationResult Validate(ValidationContext<TModel> context)
        {
            return context.InstanceToValidate == null
           ? new ValidationResult(new[] { new ValidationFailure(nameof(TModel), $"Request cannot be null") })
           : base.Validate(context);
        }
    }
}