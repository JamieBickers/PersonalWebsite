using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalWebsite.Models
{
    public class GifRequest : IPrivateApiRequest, IValidatableObject
    {
        public string Password { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password == null)
            {
                yield return new ValidationResult("Must have authorization details.");
            }
            if (Tags == null || Tags.Count() == 0)
            {
                yield return new ValidationResult("Must have tags.");
            }
        }
    }
}
