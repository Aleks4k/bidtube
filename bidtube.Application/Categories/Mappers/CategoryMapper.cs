using bidtube.Application.Categories.DTO;
using bidtube.Application.Users.DTO;
using bidtube.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Categories.Mappers
{
    [Mapper]
    public static partial class CategoryMapper
    {
        [MapProperty(nameof(Category.ID), nameof(CategoryDto.key))]
        [MapProperty(nameof(Category.name), nameof(CategoryDto.label))]
        [MapProperty(nameof(Category.Subcategories), nameof(CategoryDto.children))]
        public static partial CategoryDto ToDetailsDto(this Category entity);
    }
}
