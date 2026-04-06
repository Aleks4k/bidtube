using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Mappers;
using bidtube.Domain.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bidtube.Infrastructure.Repository
{
    public class UserRepository : IUser
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> IsUserRegistered(UserCreateDto user)
        {
            if (String.IsNullOrEmpty(user.phone))
            {
                var result = await _context.Users.AsNoTracking().Where(x => x.username == user.username || x.mail == user.mail).FirstOrDefaultAsync();
                return result != null;
            }
            else
            {
                var result = await _context.Users.AsNoTracking().Where(x => x.username == user.username || x.mail == user.mail || x.phone == user.phone).FirstOrDefaultAsync();
                return result != null;
            }
        }
        public async Task<UserDetailsDto> RegisterUser(UserCreateDto user, CancellationToken cancellationToken)
        {
            var entity = user.ToEntity();
            if (string.IsNullOrEmpty(entity.phone))
            {
                entity.phone = null;
            }
            await _context.Users.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.ToDetailsDto();
        }
        public async Task<UserDetailsDto?> GetUserByEmail(string email)
        {
            var response = await _context.Users.AsNoTracking().Where(x => x.mail == email).FirstOrDefaultAsync();
            return response == null ? null : response.ToDetailsDto();
        }
        public async Task<string> GetUserPasswordByEmail(string email)
        {
            var response = await _context.Users.AsNoTracking().Where(x => x.mail == email).FirstOrDefaultAsync();
            return response != null ? response.password : "";
        }
        public async Task UpdateGoogleUserRegistrationData(GoogleLoginFinishRegistrationDto user)
        {
            var entity = await _context.Users.Where(x => x.mail == user.mail).FirstOrDefaultAsync();
            if(entity != null)
            {
                entity.username = user.username;
                entity.password = user.password;
                entity.phone = String.IsNullOrEmpty(user.phone) ? null : user.phone;
                entity.date_of_birth = user.date_of_birth;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> IsPhoneRegistered(string? phone)
        {
            if(phone == null || string.IsNullOrEmpty(phone)) return false;
            var result = await _context.Users.AsNoTracking().Where(x => x.phone == phone).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<bool> IsUsernameRegistered(string username)
        {
            var result = await _context.Users.AsNoTracking().Where(x => x.username == username).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<bool> IsMailRegistered(string mail)
        {
            var result = await _context.Users.AsNoTracking().Where(x => x.mail == mail).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<bool> IsLoginCorrect(UserLoginDto user)
        {
            var result = await _context.Users.AsNoTracking().Where(x => x.mail == user.mail && x.password == user.password).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<UserBasicDataDto> GetUserBasicDataDto(UserGetBasicDataDto user)
        {
            var result = await _context.Users.AsNoTracking()
                .Where(x => x.mail == user.mail)
                .Select(x => new UserBasicDataDto { 
                    average_rating = x.ReviewsReceived.Any() ? x.ReviewsReceived.Average(r => r.rating) : 0,
                    total_reviews = x.ReviewsReceived.Count,
                    posted_auctions = x.Auctions.Count + x.Auctions_history.Count,
                    auctions_won = x.AuctionsWon.Count
                })
                .FirstOrDefaultAsync();
            return result!; //Na validatoru smo proverili da li je mail registrovan.
        }
        public async Task<UserBasicEditDataDto> GetUserBasicEditDataDto(UserGetBasicEditDataDto user)
        {
            var userEntity = await _context.Users.AsNoTracking().Where(x => x.mail == user.mail).FirstOrDefaultAsync();
            return new UserBasicEditDataDto
            {
                username = userEntity!.username,
                date_of_birth = userEntity!.date_of_birth,
                phone = userEntity!.phone,
                msg_allowed_from_anyone = userEntity!.msg_allowed_from_anyone
            };
        }
        public async Task<bool> IsNickOrPhoneInUseOnDifferentAcc(UpdateUserDataDto user)
        {
            if (String.IsNullOrEmpty(user.phone))
            {
                var result = await _context.Users.AsNoTracking().Where(x => x.username == user.username && x.mail != user.mail).FirstOrDefaultAsync();
                return result != null;
            } 
            else
            {
                var result = await _context.Users.AsNoTracking().Where(x => (x.username == user.username && x.mail != user.mail) || (x.phone == user.phone && x.mail != user.mail)).FirstOrDefaultAsync();
                return result != null;
            }
        }
        public async Task UpdateUserData(UpdateUserDataDto user)
        {
            var entity = await _context.Users.Where(x => x.mail == user.mail).FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.username = user.username;
                entity.phone = String.IsNullOrEmpty(user.phone) ? null : user.phone;
                entity.date_of_birth = user.date_of_birth.HasValue ? user.date_of_birth : null;
                entity.msg_allowed_from_anyone = user.msg_allowed_from_anyone;
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateUserPassword(UpdateUserPasswordDto user)
        {
            var entity = await _context.Users.Where(x => x.mail == user.mail).FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.password = user.new_password;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<string> GetUserUsernameByID(int ID)
        {
            var response = await _context.Users.AsNoTracking().Where(x => x.ID == ID).FirstOrDefaultAsync();
            return response != null ? response.username : string.Empty;
        }
    }
}
