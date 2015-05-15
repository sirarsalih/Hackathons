using Itera.Fagdag.November.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itera.Fagdag.November.Services.Contracts
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
        Product GetProduct(int id);
        void Update(Product product);
        void Remove(Product product);
        void Add(Product product);
    }
}
