using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC5.Models
{
    public class Photosets
    {
        public int cancreate { get; set; }
        public int page { get; set; }
        public int pages { get; set; }
        public int perpage { get; set; }
        public int total { get; set; }
        public List<Photoset> photoset { get; set; }
    }
}