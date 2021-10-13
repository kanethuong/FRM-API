using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.Helper
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Validate a data annotated object
        /// </summary>
        /// <param name="obj">the object</param>
        /// <param name="results">the list of error results, if failed to validate</param>
        /// <returns>Whether the object is validated successfully</returns>
        public static bool Validate(object obj, out List<ValidationResult> results)
        {
            ValidationContext context = new ValidationContext(obj, null, null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, context, results, true);
        }
    }
}