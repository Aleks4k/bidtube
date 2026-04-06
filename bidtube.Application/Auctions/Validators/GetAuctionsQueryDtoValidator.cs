using bidtube.Application.Auctions.DTO;
using bidtube.Application.Categories.Contracts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.Validators
{
    public class GetAuctionsQueryDtoValidator : AbstractValidator<GetAuctionsQueryDto>
    {
        private readonly ICategory _categoryRepository;
        public GetAuctionsQueryDtoValidator(ICategory categoryRepository)
        {
            _categoryRepository = categoryRepository;
            RuleFor(x => x.rows_to_skip).GreaterThanOrEqualTo(0).WithMessage("Rows needs to be at least 0.").Must(number => number % 10 == 0).WithMessage("Rows must be divisible by 10."); ;
            RuleFor(x => x).Must(validateGetAuctionsDto).WithMessage("Object is not in valid format.");
            RuleFor(x => x.category_id).NotEmpty().WithMessage("Category is required.").MustAsync((x, cancellation) => doesCategoryNotExist(x!.Value)).WithMessage("This category does not exist.");
        }
        private bool validateGetAuctionsDto(GetAuctionsQueryDto auction)
        {
            if(auction.sort_type == Enum.SortType.Price && auction.price_filter != 0 && auction.auction_id_filter == 0)
            {
                return false;
            }
            if (auction.sort_type == Enum.SortType.AlmostFinished && auction.date_of_expiration_filter.HasValue && auction.auction_id_filter == 0)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> doesCategoryNotExist(int category_id)
        {
            if (category_id == 0) return true; //Kod ovih komandi je moguće poslati ID 0 kao default odnosno kada user nije filtrirao stoga mi prihvatamo i ID 0 na ovom validatoru.
            var task = await _categoryRepository.DoesCategoryExist(category_id);
            return task;
        }
    }
}
