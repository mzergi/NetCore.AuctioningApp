using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AuctioningApp.Domain.RepositoryInterfaces;
using AuctioningApp.Infrastructure.Context;
using AuctioningApp.Domain.Models.DBM;

namespace AuctioningApp.Infrastructure.MSSQL_Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly MSSQL_Context db;

        public ProductsRepository(MSSQL_Context db)
        {
            this.db = db;
        }

        public async void DeleteProduct(int id)
        {
            var product = await db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ID == id);

            if(product != null)
            {
                db.Products.Remove(product);

                await db.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = await db.Products.Include(p => p.Category).ToListAsync();

            return products;
        }

        public async Task<Product> GetProduct(int id)
        {
            var product = await db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ID == id);

            return product;
        }

        public async Task<Product> PostProduct(Product product)
        {
            if (product != null)
            {
                await db.Products.AddAsync(product);

                await db.SaveChangesAsync();

                return product;
            }
            else
            {
                throw new ArgumentException("Product must not be null!");
            }
        }
    }
}
