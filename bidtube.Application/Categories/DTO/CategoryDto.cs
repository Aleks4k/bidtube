using bidtube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Categories.DTO
{
    public class CategoryDto
    {
        public int key { get; set; }
        public required string label { get; set; }
        public int? parent_category_id { get; set; }
        public string? icon_name { get; set; }
        public ICollection<CategoryDto> children { get; set; } = new List<CategoryDto>();
    }
}
