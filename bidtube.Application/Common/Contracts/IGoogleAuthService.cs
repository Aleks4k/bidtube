using bidtube.Application.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Common.Contracts
{
    public interface IGoogleAuthService
    {
        Task<bool> ValidateGoogleToken(string token);
        Task<GooglePayloadDto> GetGooglePayload(string token);
    }
}
