using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Itera.Fagdag.November.Data;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Repositories.Contracts;

namespace Itera.Fagdag.November.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly IteraLibraryDbContext _dbContext;

        public SearchRepository(IteraLibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> SearchProducts(string input)
        {
            return
                _dbContext.Products.Where(
                    x =>
                        x.Name.Contains(input) || x.Brand.Contains(input) || x.Description.Contains(input) ||
                        x.Number.Contains(input)).ToList();
        }
    }
}