﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalWebsite.Models
{
    public class RequestForAllGifRequests : IPrivateApiRequest, IValidatableObject
    {
        public AuthorizationDetails AuthorizationDetails { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AuthorizationDetails == null)
            {
                yield return new ValidationResult("Must have authorization details.");
            }
        }
    }
}