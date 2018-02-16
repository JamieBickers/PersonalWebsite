using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalWebsite.Models
{
    public class Project
    {
        public string Title { get; set; }
        public string What { get; set; }
        public string Why { get; set; }
        public IEnumerable<string> HowToUse { get; set; }
        public IEnumerable<string> Thoughts { get; set; }
        public IEnumerable<Technology> LanguagesAndLibraries { get; set; }
    }
}
