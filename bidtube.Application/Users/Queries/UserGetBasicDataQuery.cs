using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace bidtube.Application.Users.Queries
{
    public class UserGetBasicDataQueryValidator : AbstractValidator<UserGetBasicDataQuery>
    {
        public UserGetBasicDataQueryValidator(IJwtService _jwtService, IHttpContextAccessor _httpContextAccessor, IUser _userRepo)
        {
            RuleFor(x => x.User).SetValidator(new UserGetBasicDataMailValidator(_jwtService, _httpContextAccessor, _userRepo));
        }
    }
    public class UserGetBasicDataQuery: IRequest<UserBasicDataDto>
    {
        public required UserGetBasicDataDto User { get; set; }
        public UserGetBasicDataQuery() { }
        public class UserGetBasicDataQueryHandler : IRequestHandler<UserGetBasicDataQuery, UserBasicDataDto>
        {
            private readonly IUser _userRepo;
            public UserGetBasicDataQueryHandler(IUser userRepository)
            {
                _userRepo = userRepository;
            }
            public async Task<UserBasicDataDto> Handle(UserGetBasicDataQuery request, CancellationToken cancellationToken)
            {
                return await _userRepo.GetUserBasicDataDto(request.User);
            }
        }
    }
}
