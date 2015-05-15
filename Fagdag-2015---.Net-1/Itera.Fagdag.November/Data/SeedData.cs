using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Web;
using Itera.Fagdag.November.Domain.Models;

namespace Itera.Fagdag.November.Data
{
    public static class SeedData
    {
        public static void SeedProducts(IteraLibraryDbContext dbContext)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames  = assembly.GetManifestResourceNames();
            var products = new List<Product>();
            foreach (var resourceName in resourceNames)
            {
                if (resourceName.EndsWith(".txt"))
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (TextReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            var text = reader.ReadToEnd();
                            var splitted = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                            products.Add(new Product
                            {
                                Price = Convert.ToDouble(splitted[0]),
                                Name = splitted[1],
                                Description = splitted[2],
                                ImageName = resourceName.Substring(36).Replace("txt","jpg"),
                                
                                Brand = splitted[3],
                                Variant = splitted[4],
                                SizeMin = Convert.ToInt32(splitted[5]),
                                SizeMax = Convert.ToInt32(splitted[6]),
                                Number = resourceName.Substring(36).Replace(".txt", ""),

                            });                    
                        }
                    }
                }
            }

            foreach (var product in products)
            {
                dbContext.Products.Add(product);
            }
            
        }
    }
}