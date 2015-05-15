using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Repositories.Contracts;
using Itera.Fagdag.November.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Itera.Fagdag.November.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _productRepository.GetProducts();
        }
        
        public Product GetProduct(int id)
        {
            return _productRepository.GetProduct(id);
        }

        public void Update(Product product)
        {
            _productRepository.Update(product);
        }

        public void Remove(Product product)
        {
            _productRepository.Remove(product);
        }

        public void Add(Product product)
        {
            _productRepository.Add(product);
        }
    }
}