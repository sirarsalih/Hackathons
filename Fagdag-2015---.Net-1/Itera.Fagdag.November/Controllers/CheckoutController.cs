using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Mvc;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Services.Contracts;
using Itera.Fagdag.November.ViewModels;

namespace Itera.Fagdag.November.Controllers
{
    [Authorize]
    public class CheckoutController : Common
    {
        private readonly ICartService _cartService;

        public CheckoutController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public ActionResult Index()
        {
            //var userId = GetUserId();
            //var cart = _cartService.GetCartByUserId(userId) ?? CreateNewEmptyCart(userId);
            var cartViewModel = Session["Cart"] as CartViewModel ?? new CartViewModel { CartProducts = new Collection<CartProduct>()};

            //var cartViewModel = new Cart {CartProducts = new Collection<CartProduct>()};
            //cartViewModel.CartProducts = new List<CartProduct>
            //{

            //    new CartProduct {Id=12, Brand = "MyBrand2", Count = 1, Price = 43.50, Name = "Sporty overgangssko", ImageName = "1.jpg"},
            //    new CartProduct {Id=13,Brand = "MyBrand2", Count = 1, Price = 62.50, Name = "Sporty 111o", ImageName = "100.jpg"},
            //    new CartProduct {Id=14,Brand = "MyBrand2", Count = 1, Price = 93.00, Name = "Sporty 222", ImageName = "102.jpg"},
            //    new CartProduct {Id=15,Brand = "MyBrand2", Count = 1, Price = 38.00, Name = "Sporty 3333", ImageName = "103.jpg"},

            //};

            return View(cartViewModel.CartProducts);
        }

        private static Cart CreateNewEmptyCart(string userId)
        {
            return new Cart()
            {
                UserId = userId,
                CartProducts = new Collection<CartProduct>()
            };
        }

        public ActionResult KlarnaInvoice()
        {
            return View();
        }
        public ActionResult KlarnaAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteItem(int itemId)
        {
            

            return null;
        }
    }
}