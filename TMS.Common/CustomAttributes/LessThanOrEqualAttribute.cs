using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace TMS.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LessThanOrEqualAttribute : ValidationAttribute
    {
        private readonly string otherPropertyName;

        public LessThanOrEqualAttribute(string otherPropertyName)
        {
            this.otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(otherPropertyName);

            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Property with name {otherPropertyName} not found");
            }

            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (value == null || otherValue == null)
            {
                return ValidationResult.Success; // Let the Required attribute handle null values
            }

            if (Comparer.Default.Compare(value, otherValue) > 0)
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    return new ValidationResult(ErrorMessage);
                }
                return new ValidationResult($"{validationContext.DisplayName} must be less than or equal to {otherPropertyName}");
            }

            return ValidationResult.Success;
        }
    }
}
