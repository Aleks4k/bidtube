using bidtube.Application.Users.DTO;
using bidtube.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.Mappers
{
    [Mapper]
    public static partial class UserMapper
    {
        public static partial User ToEntity(this UserCreateDto dto);
        public static partial UserDetailsDto ToDetailsDto(this User entity);
    }
}
