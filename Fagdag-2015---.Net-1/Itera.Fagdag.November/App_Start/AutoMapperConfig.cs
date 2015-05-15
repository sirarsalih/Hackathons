using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.ViewModels;

namespace Itera.Fagdag.November.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<Product, ProductViewModel>();
            Mapper.CreateMap<ProductViewModel, Product>();
            Mapper.CreateMap<Cart, CartViewModel>();
            Mapper.CreateMap<Product, CartProduct>();
            Mapper.CreateMap<CartProduct, Product>();
        }
    }
}