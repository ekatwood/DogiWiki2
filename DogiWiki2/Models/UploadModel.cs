using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DogiWiki2.Models
{
    public class UploadModel
    {
        public string Name { get; set;}

        public string Description { get; set; }

        public string SelectedBreed { get; set; }

        public static List<String> Breed = new List<string>
        {
            "",
            "Labrador",
            "Golden Retriever",
            "Double Doodle"
        };

    }

    

    

}