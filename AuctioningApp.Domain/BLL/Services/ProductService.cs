using AuctioningApp.Domain.Models.DBM;
using AuctioningApp.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.BLL.Services
{
    public class ProductService
    {
        private readonly IProductsRepository productsRepository;

        public ProductService(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            return await productsRepository.PostProduct(product);
        }

        public async Task<Product> GetProduct(int id)
        {
            return await productsRepository.GetProduct(id);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await productsRepository.GetAllProducts();
        }
    }
}
