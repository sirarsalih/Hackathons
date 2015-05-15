using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Itera.Fagdag.November.ViewModels
{
    public class ManageProductsViewModel
    {
        [Required]
        [Display(Name = "Excel file")]
        public HttpPostedFileBase ExcelSchemaBase { get; set; }
    }
}