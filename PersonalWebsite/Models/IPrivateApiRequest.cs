﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalWebsite.Models
{
    interface IPrivateApiRequest
    {
        string Password { get; set; }
    }
}
