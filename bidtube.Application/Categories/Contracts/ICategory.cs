using bidtube.Application.Categories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Categories.Contracts
{
    public interface ICategory
    {
        Task<AllCategoriesDto> GetAllCategories();
        Task<bool> DoesCategoryExist(int category_id);
        Task<List<int>> GetSubcategoriesForCategoryId(int category_id);
    }
}
