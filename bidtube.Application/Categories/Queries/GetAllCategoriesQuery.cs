using bidtube.Application.Categories.Contracts;
using bidtube.Application.Categories.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Categories.Queries
{
    public class GetAllCategoriesQuery:IRequest<AllCategoriesDto>
    {
        public GetAllCategoriesQuery() { }
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, AllCategoriesDto>
        {
            private readonly ICategory _categoryRepo;
            public GetAllCategoriesQueryHandler(ICategory categoryRepository)
            {
                _categoryRepo = categoryRepository;
            }
            public async Task<AllCategoriesDto> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
            {
                return await _categoryRepo.GetAllCategories();
            }
        }
    }
}
