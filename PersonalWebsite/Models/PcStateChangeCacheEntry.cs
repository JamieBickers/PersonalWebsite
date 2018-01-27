using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalWebsite.Models
{
    public class PcStateChangeCacheEntry
    {
        public PcStateChangeCacheEntry(string action, DateTimeOffset date)
        {
            Action = action;
            Date = date;
        }

        public string Action { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
