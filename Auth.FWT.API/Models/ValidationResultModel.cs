using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Auth.FWT.API.Models
{
    public class ValidationResultModel
    {
        public ValidationResultModel(ValidationException validationException)
        {
            Message = "Validation Failed";
            Errors = validationException.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList();
        }

        public List<ValidationError> Errors { get; }

        public string Message { get; }
    }
}
