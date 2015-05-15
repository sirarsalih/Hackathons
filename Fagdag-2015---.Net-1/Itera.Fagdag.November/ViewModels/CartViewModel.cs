using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Itera.Fagdag.November.Domain.Models;

namespace Itera.Fagdag.November.ViewModels
{
    public class CartViewModel
    {
        public ICollection<CartProduct> CartProducts { get; set; }

        public string CartText
        {
            get
            {
                if (CartProducts.Count == 0)
                {
                    return "Your cart is empty.";
                }
                return string.Empty;
            }
        }
    }
}