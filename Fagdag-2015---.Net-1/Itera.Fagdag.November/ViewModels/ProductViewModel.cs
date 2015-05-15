using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Itera.Fagdag.November.Controllers;

namespace Itera.Fagdag.November.ViewModels
{
    public class ProductViewModel
    {
        public int Number { get; set; }
        public string LoggedInUserName { get; set; }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string ShortDescription
        {
            get
            {
                var shortDesc = Description.Substring(0, Description.Length/2);
                return shortDesc += "...";
            }
        }

        public string ImageName { get; set; }

        [Required(ErrorMessage = "Please upload a cover image.")]
        [Display(Name = "Cover")]
        public HttpPostedFileBase CoverImageBase { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public string Language { get; set; }
        
        [Required]
        public string Brand { get; set; }

        [Required]
        public double Price { get; set; }
        
        [Required]
        public int SizeMin { get; set; }

        [Required]
        public int SizeMax { get; set; }

        [Required]
        public string Variant { get; set; }

    }
}