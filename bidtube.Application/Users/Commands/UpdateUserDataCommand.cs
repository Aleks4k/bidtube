using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace bidtube.Application.Users.Commands
{
    public class UpdateUserDataCommandValidator : AbstractValidator<UpdateUserDataCommand>
    {
        public UpdateUserDataCommandValidator(IJwtService _jwtService, IHttpContextAccessor _httpContextAccessor, IUser _userRepo)
        {
            RuleFor(x => x.User).SetValidator(new UpdateUserDataDtoValidator(_jwtService, _httpContextAccessor, _userRepo));
        }
    }
    public class UpdateUserDataCommand : IRequest<Unit>
    {
        public required UpdateUserDataDto User { get; set; }
        public UpdateUserDataCommand(){}
        public class UpdateUserDataCommandHandler: IRequestHandler<UpdateUserDataCommand, Unit>
        {
            private readonly IUser _userRepo;
            public UpdateUserDataCommandHandler(IUser userRepository)
            {
                _userRepo = userRepository;
            }
            public async Task<Unit> Handle(UpdateUserDataCommand request, CancellationToken cancellationToken)
            {
                await _userRepo.UpdateUserData(request.User);
                return Unit.Value;
            }
        }
    }
}
