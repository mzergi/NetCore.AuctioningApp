using AuctioningApp.Domain.Models.DBM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.BLL.ServiceInterfaces
{
    public interface ICategoriesService
    {
        public Task<List<Category>> GetAllCategories();

        public Task<Category> GetCategory(int id);

        public Task<Category> PostCategory(Category category, string parentid);

        public Task<Category> DeleteCategory(Category category);
    }
}
