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
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly MSSQL_Context db;

        public CategoriesRepository (MSSQL_Context db)
        {
            this.db = db;
        }

        public async void DeleteCategory(int id)
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.ID == id);
            if(category != null)
            {
                db.Categories.Remove(category);

                await db.SaveChangesAsync();
            }
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var categories = await db.Categories.ToListAsync();

            return categories;
        }

        public async Task<Category> GetCategory(int id)
        {
            var category = await db.Categories.FirstOrDefaultAsync(p => p.ID == id);

            return category;
        }

        public async Task<Category> PostCategory(Category category)
        {

            if (db.Categories.Any(s => EF.Functions.Like(s.Name, category.Name)))
                throw new ArgumentException("Name must be unique!");

            await db.Categories.AddAsync(category);

            var success = await db.SaveChangesAsync();

            return category;
        }
    }
}
