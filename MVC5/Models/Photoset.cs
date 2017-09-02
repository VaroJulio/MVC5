using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC5.Models
{
    public class Photoset
    {
        public string id { get; set; }
        public string primary { get; set; }
        public string secret { get; set; }
        public string server { get; set; }
        public int farm { get; set; }
        public int photos { get; set; }
        public int videos { get; set; }
        public Title title { get; set; }
        public Description description { get; set; }
        public int needs_interstitial { get; set; }
        public int visibility_can_see_set { get; set; }
        public int count_views { get; set; }
        public int count_comments { get; set; }
        public int can_comment { get; set; }
        public string date_create { get; set; }
        public int date_update { get; set; }
    }
}