using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Logging.Contract;
using Itera.Fagdag.November.Services.Contracts;
using Itera.Fagdag.November.ViewModels;

namespace Itera.Fagdag.November.Controllers
{
    public class ProductController : Common
    {
        private readonly IProductService _productService;


        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public ActionResult Index()
        {
            var products = _productService.GetProducts();
            if (products == null)
            {
                return View("~/Views/Common/NotFound.cshtml", new NotFoundViewModel(){ElementName = "Products"});
            }
            var productViewModels = products.Select(Mapper.Map<Product, ProductViewModel>).ToList();
            return View(productViewModels);
        }
        public ActionResult Details(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
            {
                return View("~/Views/Common/NotFound.cshtml", new NotFoundViewModel(){ElementName = "Product"});
            }
            var productViewModel = Mapper.Map<Product, ProductViewModel>(product);
            productViewModel.LoggedInUserName = GetUserName();
            return View(productViewModel);
        }

        public ActionResult ReserveAndUpdateCart(int id)
        {
            //var userId = GetUserId();
            //var cart = _cartService.GetCartByUserId(userId);
            var product = _productService.GetProduct(id);

            //if (product != null)
            //{
            var cart = Session["Cart"] as CartViewModel;
            if (cart == null)
            {
                cart = new CartViewModel {CartProducts = new List<CartProduct>()};
            }

            var currentProduct = cart.CartProducts.FirstOrDefault(x => x.Number == product.Number);
            if (currentProduct != null)
            {
                currentProduct.Count++;
            }
            else
            {
                cart.CartProducts.Add(new CartProduct
                {
                    Count = 1,
                    Description = product.Description,
                    Brand = product.Brand,
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ImageName = product.ImageName,
                    SizeMin = product.SizeMin,
                    SizeMax = product.SizeMax,
                    Variant = product.Variant
                });
            }

            Session["Cart"] = cart;

            var productViewModel = Mapper.Map<Product, ProductViewModel>(product);
            productViewModel.LoggedInUserName = GetUserName();
            return RedirectToAction("Details", productViewModel);
        }
    }
}