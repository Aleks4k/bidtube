using bidtube.Application.Categories.Contracts;
using bidtube.Application.Categories.DTO;
using bidtube.Application.Categories.Mappers;
using bidtube.Domain.Data;
using bidtube.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Infrastructure.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> DoesCategoryExist(int category_id)
        {
            var response = await _context.Categories.AsNoTracking().Where(x => x.ID == category_id).FirstOrDefaultAsync();
            return response != null;
        }
        public async Task<AllCategoriesDto> GetAllCategories()
        {
            var response = await _context.Categories.AsNoTracking().ToListAsync();
            var rootCategories = response.Where(x => x.parent_category_id == null).ToList();
            var categoryTree = BuildCategoryTree(rootCategories, response); //Efikasno rešenje da ne pozivamo puno rekurzija po bazi nego ih rešavamo u kodu.
            var responseDtos = categoryTree.Select(x => x.ToDetailsDto()).ToList();
            return new AllCategoriesDto {
                categories = responseDtos
            };
        }
        public async Task<List<int>> GetSubcategoriesForCategoryId(int category_id)
        {
            var entity = await _context.Categories.AsNoTracking().ToListAsync();
            var rootCategory = entity.Where(x => x.ID == category_id).FirstOrDefault();
            var response = BuildCategoryIterator(rootCategory!, entity); //! provereno na validatoru.
            return response;
        }
        private List<Category> BuildCategoryTree(List<Category> rootCategories, List<Category> allCategories)
        {
            foreach (var category in rootCategories)
            {
                category.Subcategories = allCategories
                    .Where(c => c.parent_category_id == category.ID)
                    .ToList();
                BuildCategoryTree(category.Subcategories.ToList(), allCategories);
            }
            return rootCategories;
        }
        private List<int> BuildCategoryIterator(Category category, List<Category> allCategories)
        {
            var resp = new List<int>();
            resp.Add(category.ID);
            foreach(var subcategory in allCategories)
            {
                if(subcategory.parent_category_id == category.ID)
                {
                    resp.AddRange(BuildCategoryIterator(subcategory, allCategories));
                }
            }
            return resp;
        }
    }
}
