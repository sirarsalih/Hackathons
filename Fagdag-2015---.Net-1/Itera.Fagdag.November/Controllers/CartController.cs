using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Services.Contracts;
using Itera.Fagdag.November.Validation.Contracts;
using Itera.Fagdag.November.ViewModels;
using Microsoft.AspNet.Identity;

namespace Itera.Fagdag.November.Controllers
{
    [Authorize]
    public class CartController : Common
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public CartController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public ActionResult Index()
        {
            var userId = GetUserId();
            var cart = _cartService.GetCartByUserId(userId) ?? CreateNewEmptyCart(userId);
            var cartViewModel = Mapper.Map<Cart, CartViewModel>(cart);
            return View("Details", cartViewModel);
        }

        private static Cart CreateNewEmptyCart(string userId)
        {
            return new Cart()
            {
                UserId = userId,
                CartProducts = new Collection<CartProduct>()
            };
        }

        //public ActionResult UnreserveAndUpdateCart(string number)
        //{
        //    var userId = GetUserId();
        //    var cart = _cartService.GetCartByUserId(userId);

        //    if (cart == null)
        //    {
        //        return View("~/Views/Common/NotFound.cshtml", new NotFoundViewModel() {ElementName = "User cart"});
        //    }

        //    var cartProduct = cart.CartProducts.FirstOrDefault(x => x.Number == number);

        //    if (cartProduct != null)
        //    {
        //        var product = Mapper.Map<CartProduct, Product>(cartProduct);

        //        if (!_productValidation.ProductIsAlreadyUnreserved(product))
        //        {
        //            Unreserve(product);
        //            RemoveProductFromCart(cart, cartProduct);
        //        }
        //    }
        //    var cartViewModel = Mapper.Map<Cart, CartViewModel>(cart);
        //    return View("Details", cartViewModel);
        //}

        //private void Unreserve(Product product)
        //{
        //    product.ReservedBy = null;
        //    _productService.Update(product);
        //}

        private void RemoveProductFromCart(Cart cart, CartProduct cartProduct)
        {
            cart.CartProducts.Remove(cartProduct);
            _cartService.UpdateCart(cart);
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
                    cart = new CartViewModel { CartProducts = new List<CartProduct>() };
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
                        Description = currentProduct.Description,
                        Brand = currentProduct.Brand,
                        Id = currentProduct.Id,
                        Name = currentProduct.Name,
                        Price = currentProduct.Price,
                        ImageName = currentProduct.ImageName,
                        SizeMin = currentProduct.SizeMin,
                        SizeMax = currentProduct.SizeMax,
                        Variant = currentProduct.Variant
                    });
                }

                Session["Cart"] = cart;
                //        if (!_productValidation.ProductIsAlreadyReserved(product))
                //        {
                //            Reserve(product);
                //            var cartProduct = Mapper.Map<Product, CartProduct>(product);

                //            if (cart == null)
                //            {
                //                cart = CreateNewCartWithProduct(userId, cartProduct);
                //            }
                //            else
                //            {
                //                AddNewProductToCart(cart, cartProduct);
                //            }
                //        }
                //    }

                var productViewModel = Mapper.Map<Product, ProductViewModel>(product);
                productViewModel.LoggedInUserName = GetUserName();
                return View(productViewModel);

                //var cartViewModel = Mapper.Map<Cart, CartViewModel>(cart);

                //return View("Details", cartViewModel);
            //}
            

        }

        //private void Reserve(Product product)
        //{
        //    product.ReservedBy = GetUserName();
        //    _productService.Update(product);
        //}

        private void AddNewProductToCart(Cart cart, CartProduct cartProduct)
        {
            if (cart.CartProducts.FirstOrDefault(x => x.Number == cartProduct.Number) == null)
            {
                cart.CartProducts.Add(cartProduct);
                _cartService.UpdateCart(cart);
            }
        }

        private Cart CreateNewCartWithProduct(string userId, CartProduct cartProduct)
        {
            var newCart = new Cart()
            {
                UserId = userId,
                CartProducts = new Collection<CartProduct>()
                {
                    cartProduct
                }
            };
            _cartService.InsertCart(newCart);
            return newCart;
        }
    }
}