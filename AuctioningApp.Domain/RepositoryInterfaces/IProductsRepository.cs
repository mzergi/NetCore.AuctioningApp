using AuctioningApp.Domain.Models.DBM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.RepositoryInterfaces
{
    public interface IProductsRepository
    {
        public Task<List<Product>> GetAllProducts();

        public Task<Product> GetProduct(int id);

        public Task<Product> PostProduct(Product product);

        public void DeleteProduct(int id);
    }
}
