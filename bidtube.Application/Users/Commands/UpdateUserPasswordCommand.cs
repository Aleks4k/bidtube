using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.Commands
{
    public class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
    {
        public UpdateUserPasswordCommandValidator(IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IUser userRepo)
        {
            RuleFor(x => x.User).SetValidator(new UpdateUserPasswordDtoValidator(jwtService, httpContextAccessor, userRepo));
        }
    }
    public class UpdateUserPasswordCommand : IRequest<Unit>
    {
        public required UpdateUserPasswordDto User { get; set; }
        public UpdateUserPasswordCommand() { }
        public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand, Unit>
        {
            private readonly IUser _userRepo;
            public UpdateUserPasswordCommandHandler(IUser userRepository)
            {
                _userRepo = userRepository;
            }
            public async Task<Unit> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
            {
                await _userRepo.UpdateUserPassword(request.User);
                return Unit.Value;
            }
        }
    }
}
