using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Helpers;
using Itera.Fagdag.November.Logging.Contract;
using Itera.Fagdag.November.Resources.Keys;
using Itera.Fagdag.November.Services.Contracts;
using Itera.Fagdag.November.Validation.Contracts;
using Itera.Fagdag.November.ViewModels;
using OfficeOpenXml;

namespace Itera.Fagdag.November.Controllers
{
    [Authorize]
    public class ManageProductsController : Common
    {
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IFileFormatValidation _fileFormatValidation;

        public ManageProductsController(ILogger logger, IProductService productService,
            IFileFormatValidation fileFormatValidation)
        {
            _logger = logger;
            _productService = productService;
            _fileFormatValidation = fileFormatValidation;
        }

        public ActionResult Index()
        {
            var products = _productService.GetProducts();
            if (products == null)
            {
                return View("~/Views/Common/NotFound.cshtml", new NotFoundViewModel() {ElementName = "Products"});
            }
            var productViewModels = products.Select(Mapper.Map<Product, ProductViewModel>).ToList();
            return View(productViewModels);
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult AddMany()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProducts(ManageProductsViewModel manageProductsViewModel)
        {
            if (manageProductsViewModel == null)
            {
                return View("~/Views/Common/NotFound.cshtml", new NotFoundViewModel() {ElementName = "Products"});
            }

            if (ModelState.IsValid)
            {
                if (ModelState.Values.Any(x => x.Errors.Count >= 1))
                {
                    return View("AddMany", manageProductsViewModel);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please upload an Excel file with product information.");
                return View("AddMany", manageProductsViewModel);
            }
            return RedirectToAction("index");
        }

        
        public ActionResult RemoveProduct(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
            {
                return View("~/Views/Common/NotFound.cshtml", new NotFoundViewModel() { ElementName = "Product" });
            }
            _productService.Remove(product);
            return RedirectToAction("Index", "ManageProducts");
        }

        [HttpPost]
        public ActionResult AddProduct(ProductViewModel productViewModel)
        {
            if (productViewModel == null)
            {
                return View("~/Views/Common/NotFound.cshtml", new NotFoundViewModel() {ElementName = "Product"});
            }

            if (ModelState.IsValid)
            {
                var product = Mapper.Map<ProductViewModel, Product>(productViewModel);
                SaveCoverImageOnServer(productViewModel, product);
                if (ModelState.Values.Any(x => x.Errors.Count >= 1))
                {
                    return View("Add", productViewModel);
                }
                _productService.Add(product);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The fields below must be filled.");
                return View("Add", productViewModel);
            }

            return RedirectToAction("index");
        }

        private void SaveCoverImageOnServer(ProductViewModel productViewModel, Product product)
        {
            var fileName = productViewModel.CoverImageBase.FileName;
            if (_fileFormatValidation.IsImageType(fileName))
            {
                SaveOnServer(productViewModel.CoverImageBase.FileName, Keys.CoverImageBase);
                product.ImageName = String.Concat(Keys.PathUploads + "/", productViewModel.CoverImageBase.FileName);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please upload a valid image file.");
            }
        }

        [HttpPost]
        public ActionResult UpdateProduct(string id)
        {
            var product = _productService.GetProduct(id.ToInt32());
            if (product == null)
            {
                return View("~/Views/Common/NotFound.cshtml", new NotFoundViewModel() { ElementName = "Product" });
            }

            SaveCoverImageOnServer(product);
            _productService.Update(product);

            return RedirectToAction("Index");
        }

        private void SaveCoverImageOnServer(Product product)
        {
            foreach (var file in from string item in Request.Files select Request.Files[item])
            {
                SaveOnServer(file.FileName, Keys.CoverImageBase);
                product.ImageName = String.Concat(Keys.PathUploads + "/", file.FileName);
                break;
            }
        }
    }
}