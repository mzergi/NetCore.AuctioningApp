using AuctioningApp.Domain.BLL.ServiceInterfaces;
using AuctioningApp.Domain.Models.DBM;
using AuctioningApp.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.BLL.Services
{
    public class CategoriesService : ICategoriesService
    {

        private readonly ICategoriesRepository categoriesRepository;

        public CategoriesService(ICategoriesRepository categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public async Task<Category> DeleteCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await categoriesRepository.GetAllCategories();
        }

        public async Task<Category> GetCategory(int id)
        {
            return await categoriesRepository.GetCategory(id);
        }

        public async Task<Category> PostCategory(Category category, string parentid)
        {
            if(category == null)
            {
                throw new ArgumentException("Category must not be null!");
            }

            if (parentid.Equals("null"))
            {
                try
                {
                    var result = await categoriesRepository.PostCategory(category);

                    return result;
                }

                catch (ArgumentException ex)
                {
                    throw ex;
                }
            }
            else
            {
                if (int.Parse(parentid) > 0)
                {
                    var parent = await categoriesRepository.GetCategory(int.Parse(parentid));
                    if(parent == null)
                    {
                        throw new ArgumentException("Parent ID not valid!");
                    }
                    else
                    {
                        try
                        {
                            var result = await categoriesRepository.PostCategory(category);

                            return result;
                        }

                        catch (ArgumentException ex)
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("ID must be positive!");
                }
            }
        }
    }
}
