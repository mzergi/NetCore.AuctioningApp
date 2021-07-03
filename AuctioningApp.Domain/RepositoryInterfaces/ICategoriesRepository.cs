using AuctioningApp.Domain.Models.DBM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.RepositoryInterfaces
{
    public interface ICategoriesRepository
    {
        public Task<List<Category>> GetAllCategories();

        public Task<Category> GetCategory(int id);

        public Task<Category> PostCategory(Category category);

        public void DeleteCategory(int id);

    }
}
