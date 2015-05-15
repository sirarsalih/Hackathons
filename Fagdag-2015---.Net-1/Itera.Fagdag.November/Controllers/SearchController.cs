using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Services.Contracts;
using Itera.Fagdag.November.ViewModels;

namespace Itera.Fagdag.November.Controllers
{
    public class SearchController : Common
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel searchViewModel)
        {
            if (searchViewModel.Input.Trim() == string.Empty)
            {
                return RedirectToAction("Index", "Product");
            }
            var products = _searchService.SearchProducts(searchViewModel.Input);
            var productViewModels = products.Select(Mapper.Map<Product, ProductViewModel>).ToList();
            return View("~/Views/Product/Index.cshtml", productViewModels);
        }
    }
}