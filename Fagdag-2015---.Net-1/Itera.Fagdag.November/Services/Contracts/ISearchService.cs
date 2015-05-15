using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Itera.Fagdag.November.Domain.Models;

namespace Itera.Fagdag.November.Services.Contracts
{
    public interface ISearchService
    {
        IEnumerable<Product> SearchProducts(string input);
    }
}
