using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalWebsite.Models
{
    public class GifRequestsCacheEntry
    {
        public IEnumerable<string> Tags { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
