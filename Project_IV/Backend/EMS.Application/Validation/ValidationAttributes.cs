using System.ComponentModel.DataAnnotations;

namespace EMS.Application.Validation
{
    // Validates that the decorated DateTime property is today or in the future.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute()
            : base("The {0} must be today or a future date.")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date < DateTime.UtcNow.Date)
                {
                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName),
                        new[] { validationContext.MemberName! });
                }
            }
            return ValidationResult.Success;
        }
    }

    // Class-level attribute applied to SessionDto.
    // Validates SessionEnd > SessionStart and SessionStart is not in the past.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SessionDateRangeAttribute : ValidationAttribute
    {
        private readonly string _startProperty;
        private readonly string _endProperty;

        public SessionDateRangeAttribute(string startProperty, string endProperty)
        {
            _startProperty = startProperty;
            _endProperty   = endProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var type = validationContext.ObjectType;

            var startProp = type.GetProperty(_startProperty);
            var endProp   = type.GetProperty(_endProperty);

            if (startProp == null || endProp == null)
                return ValidationResult.Success;

            var startValue = startProp.GetValue(validationContext.ObjectInstance);
            var endValue   = endProp.GetValue(validationContext.ObjectInstance);

            if (startValue is DateTime start && endValue is DateTime end)
            {
                if (end <= start)
                {
                    return new ValidationResult(
                        "SessionEnd must be strictly after SessionStart.",
                        new[] { _endProperty });
                }

                if (start.Date < DateTime.UtcNow.Date)
                {
                    return new ValidationResult(
                        "SessionStart must be today or a future date.",
                        new[] { _startProperty });
                }
            }

            return ValidationResult.Success;
        }
    }
}
