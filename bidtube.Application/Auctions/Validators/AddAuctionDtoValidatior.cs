using bidtube.Application.Auctions.DTO;
using bidtube.Application.Categories.Contracts;
using bidtube.Application.Users.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using bidtube.Application.Auctions.Contracts;
using bidtube.Application.Common.Contracts;

namespace bidtube.Application.Auctions.Validators
{
    public class AddAuctionDtoValidatior : AbstractValidator<AddAuctionDto>
    {
        private readonly IUser _userRepository;
        private readonly ICategory _categoryRepository;
        private readonly IAuction _auctionRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly string JPEGMagicNumber = "/9j/"; //JPEG SIGNATURE.
        private static readonly double maximumImageSize = 1048576; //1MB što je sasvim dovoljno pošto su slike 500x500
        public AddAuctionDtoValidatior(IUser userRepository, ICategory categoryRepository, IAuction auctionRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _auctionRepository = auctionRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            RuleFor(x => x.mail).NotEmpty().WithMessage("Mail address is required.").MaximumLength(255).WithMessage("Mail address is too long.").EmailAddress().WithMessage("Mail adress is not in valid format.").MustAsync((x, cancellation) => checkMail(x)).WithMessage("You are not allowed to post auction.").MustAsync((x, cancellation) => isMailNotRegistered(x)).WithMessage("This mail is not registered.");
            RuleFor(x => x.title).NotEmpty().WithMessage("Title is required.").MinimumLength(8).WithMessage("Title must be at least 8 characters.").MaximumLength(64).WithMessage("Title can be up to 64 characters long.");
            RuleFor(x => x.description).NotEmpty().WithMessage("Description is required.").MinimumLength(24).WithMessage("Description must be at least 24 characters.").MaximumLength(1024).WithMessage("Description can be up to 1024 characters long.");
            RuleFor(x => x.category_id).NotEmpty().WithMessage("Category is required.").MustAsync((x, cancellation) => doesCategoryNotExist(x)).WithMessage("This category does not exist.");
            RuleFor(x => x.starting_price).NotEmpty().WithMessage("Starting price is required.").GreaterThanOrEqualTo(0.50).WithMessage("Starting price needs to be at least 0,50€.").LessThanOrEqualTo(1000000000).WithMessage("Starting price cannot exceed 1,000,000,000€.");
            RuleFor(x => x.date_of_exp).NotEmpty().WithMessage("Date of expiration is required.").Must(isValidDate).WithMessage("The auction must last at least one day and no more than one year.");
            RuleFor(x => x.Image_names).NotEmpty().WithMessage("You need to upload at least one image.").Must(validateImageNamesForDuplicates).WithMessage("You can't use same file twice.").Must(validateImageCount).WithMessage("You can't upload more than 9 photos.").Must(validateImageNamesForSize).WithMessage("The name of the photo must be between 1 and 96 characters long.");
            RuleFor(x => x.Images).NotEmpty().WithMessage("You need to upload at least one image.").Must(validateImageCount).WithMessage("You can't upload more than 9 photos.").Must(MagicNumberJPEGValidation).WithMessage("Uploaded images are not in a valid format.").Must(imageSizeValidator).WithMessage("The content of the uploaded image exceeds the allowable limit.").MustAsync((x, cancellation) => validateImages(x)).WithMessage("Uploaded images are not in a valid format.");
            RuleFor(x => x).Must(checkImageArrays).WithMessage("Please reupload images again.");
        }
        private bool imageSizeValidator(List<string> images)
        {
            foreach(var image in images)
            {
                if(image.Length > maximumImageSize)
                {
                    return false;
                }
            }
            return true;
        }
        private bool MagicNumberJPEGValidation(List<string> images)
        {
            foreach(string image in images)
            {
                var b64 = image.Replace("data:application/octet-stream;base64,", "");
                if (!b64.StartsWith(JPEGMagicNumber))
                {
                    return false;
                }
            }
            return true;
        }
        private bool checkImageArrays(AddAuctionDto auction)
        {
            if(auction.Images.Count != auction.Image_names.Count)
            {
                return false;
            }
            return true;
        }
        private bool validateImageNamesForSize(List<string> image_names)
        {
            foreach(var image_name in image_names)
            {
                if(image_name.Length > 96)
                {
                    return false;
                }
                if(image_name.Length < 1)
                {
                    return false;
                }
            }
            return true;
        }
        private bool validateImageCount(List<string> images)
        {
            if(images.Count > 9)
            {
                return false;
            }
            return true;
        }
        private bool validateImageNamesForDuplicates(List<string> image_names)
        {
            HashSet<string> set = new HashSet<string>(image_names);
            return set.Count == image_names.Count;
        }
        private async Task<bool> validateImages(List<string> images)
        {
            var result = await _auctionRepository.ValidateImages(images);
            return result;
        }
        private async Task<bool> isMailNotRegistered(string mail)
        {
            //Ovde zapravo gledamo da li mejl NIJE registrovan i ako nije odbijamo request. Možda čak i nepotrebna provera ali nije na odmet.
            var task = await _userRepository.IsMailRegistered(mail);
            return task;
        }
        private async Task<bool> doesCategoryNotExist(int category_id)
        {
            var task = await _categoryRepository.DoesCategoryExist(category_id);
            return task;
        }
        private async Task<bool> checkMail(string mail)
        {
            //Već znamo da ima auth zato što je prošao kroz middleware i filter.
            var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
            var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            return await _jwtService.ValidateAccessToken(token!, mail);
        }
        private bool isValidDate(DateTime date_of_exp)
        {
            var minDate = DateTime.UtcNow.AddDays(1).AddHours(-1); //Stavljamo 23 sata jer dok user popuni formu predpostavljamo da neće proći više od 1 sat.
            var maxDate = DateTime.UtcNow.AddYears(1).AddDays(1); //366 dana.
            if(minDate > date_of_exp || maxDate < date_of_exp)
            {
                return false;
            }
            return true;
        }
    }
}
