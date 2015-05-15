using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Itera.Fagdag.November.Domain.Models;
using Itera.Fagdag.November.Repositories.Contracts;
using Itera.Fagdag.November.Services.Contracts;

namespace Itera.Fagdag.November.Services
{
    public class SearchService : ISearchService
    {
        private readonly ISearchRepository _searchRepository;

        public SearchService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public IEnumerable<Product> SearchProducts(string input)
        {
            return _searchRepository.SearchProducts(input);
        }
    }
}