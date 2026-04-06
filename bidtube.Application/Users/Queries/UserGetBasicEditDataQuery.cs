using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace bidtube.Application.Users.Queries
{
    public class UserGetBasicEditDataQueryValidator : AbstractValidator<UserGetBasicEditDataQuery>
    {
        public UserGetBasicEditDataQueryValidator(IJwtService _jwtService, IHttpContextAccessor _httpContextAccessor, IUser _userRepo)
        {
            RuleFor(x => x.User).SetValidator(new UserGetBasicEditDataMailValidator(_jwtService, _httpContextAccessor, _userRepo));
        }
    }
    public class UserGetBasicEditDataQuery : IRequest<UserBasicEditDataDto>
    {
        public required UserGetBasicEditDataDto User { get; set; }
        public UserGetBasicEditDataQuery() { }
        public class UserGetBasicEditDataQueryHandler : IRequestHandler<UserGetBasicEditDataQuery, UserBasicEditDataDto>
        {
            private readonly IUser _userRepo;
            public UserGetBasicEditDataQueryHandler(IUser userRepository)
            {
                _userRepo = userRepository;
            }
            public async Task<UserBasicEditDataDto> Handle(UserGetBasicEditDataQuery request, CancellationToken cancellationToken)
            {
                return await _userRepo.GetUserBasicEditDataDto(request.User);
            }
        }
    }
}
