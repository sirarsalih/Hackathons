using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Itera.Fagdag.November.Data;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Itera.Fagdag.November.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IteraLibraryDbContext _dbContext;

        public ProductRepository(IteraLibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _dbContext.Products.ToList();
        }

        public Product GetProduct(int id)
        {
            return _dbContext.Products.Find(id);
        }
        public void Update(Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
            Save();
        }

        public void Remove(Product product)
        {
            _dbContext.Products.Remove(product);
            Save();
        }

        public void Add(Product product)
        {
            _dbContext.Products.Add(product);
            Save();
        }

        private void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}