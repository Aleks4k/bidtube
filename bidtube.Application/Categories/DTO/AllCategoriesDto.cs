using bidtube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Categories.DTO
{
    public class AllCategoriesDto
    {
        public List<CategoryDto> categories { get; set; } = new List<CategoryDto>();
    }
}
