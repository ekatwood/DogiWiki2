using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DogiWiki2.Models
{
    public class IndexModel
    {
        public string Filter { get; set; }

        public string SortBy { get; set; }

        public static List<String> FilterList = new List<string>
        {
            "All",
            "Labrador",
            "Golden Retriever",
            "Double Doodle"
        };

        public static List<String> SortByList = new List<string>
        {
            "Newest",
            "Most Popular",
            "Random",
        };       
    }
}