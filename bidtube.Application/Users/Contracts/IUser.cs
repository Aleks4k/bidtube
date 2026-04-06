using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.Contracts
{
    public interface IUser
    {
        Task<bool> IsUserRegistered(UserCreateDto user);
        Task<UserDetailsDto> RegisterUser(UserCreateDto user, CancellationToken cancellationToken);
        Task<UserDetailsDto?> GetUserByEmail(string email);
        Task<string> GetUserPasswordByEmail(string email);
        Task UpdateGoogleUserRegistrationData(GoogleLoginFinishRegistrationDto user);
        Task<bool> IsPhoneRegistered(string? phone);
        Task<bool> IsUsernameRegistered(string username);
        Task<bool> IsMailRegistered(string mail);
        Task<bool> IsLoginCorrect(UserLoginDto user);
        Task<UserBasicDataDto> GetUserBasicDataDto(UserGetBasicDataDto user);
        Task<UserBasicEditDataDto> GetUserBasicEditDataDto(UserGetBasicEditDataDto user);
        Task<bool> IsNickOrPhoneInUseOnDifferentAcc(UpdateUserDataDto user);
        Task UpdateUserData(UpdateUserDataDto user);
        Task UpdateUserPassword(UpdateUserPasswordDto user);
        Task<string> GetUserUsernameByID(int ID);
    }
}
