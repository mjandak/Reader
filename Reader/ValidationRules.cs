using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Reader
{
    class RequiredFiedValidationRule : ValidationRule
    {
        public RequiredFiedValidationRule()
        {

        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value.ToString().Length > 0)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "Required Field Validation");
            }
        }
    }
}
